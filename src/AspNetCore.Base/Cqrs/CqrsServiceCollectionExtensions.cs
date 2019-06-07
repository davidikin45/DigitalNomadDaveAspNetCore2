using AspNetCore.Base.Cqrs.Decorators.Command;
using AspNetCore.Base.DomainEvents.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.Base.Cqrs
{
    public static class CqrsServiceCollectionExtensions
    {
        public static void AddCqrs(this IServiceCollection services)
        {
            services.AddCqrsMediator();
            services.AddCqrsHandlers(new List<Assembly>() { Assembly.GetCallingAssembly() });
        }

        public static void AddCqrs(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddCqrsMediator();
            services.AddCqrsHandlers(assemblies);
        }

        public static void AddCqrsMediator(this IServiceCollection services)
        {
            services.TryAddTransient<ICqrsMediator, CqrsMediator>();
        }

        public static void AddCqrsHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //commands
            List<Type> commandHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsCommandHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler") && !x.IsAbstract && !x.IsGenericType)
                .ToList();

            foreach (Type commandHandlerType in commandHandlerTypes)
            {
                AddHandlerAsService(services, commandHandlerType, false);
            }

            services.AddSingleton<ICqrsCommandSubscriptionsManager>(sp => {
                var subManager = new CqrsInMemoryCommandSubscriptionsManager();
                foreach (Type commandHandlerType in commandHandlerTypes.Where(x => x.GetInterfaces().Any(y => IsCommandHandlerInterface(y))))
                {
                    IEnumerable<Type> interfaceTypes = commandHandlerType.GetInterfaces().Where(y => IsCommandHandlerInterface(y));

                    foreach (Type interfaceType in interfaceTypes)
                    {
                        Type commandType = interfaceType.GetGenericArguments()[0];

                        if (commandType != typeof(Object))
                        {
                            subManager.AddSubscription(commandType, interfaceType);
                        }
                    }
                }
                return subManager;
            });

            //queries
            List<Type> queryHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsQueryHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler") && !x.IsAbstract && !x.IsGenericType)
                .ToList();

            foreach (Type queryHandlerType in queryHandlerTypes)
            {
                AddHandlerAsService(services, queryHandlerType, true);
            }

            services.AddSingleton<ICqrsQuerySubscriptionsManager>(sp => {
                var subManager = new CqrsInMemoryQuerySubscriptionsManager();
                foreach (Type queryHandlerType in queryHandlerTypes.Where(x => x.GetInterfaces().Any(y => IsQueryHandlerInterface(y))))
                {
                    IEnumerable<Type> interfaceTypes = queryHandlerType.GetInterfaces().Where(y => IsQueryHandlerInterface(y));

                    foreach (var interfaceType in interfaceTypes)
                    {
                        Type queryType = interfaceType.GetGenericArguments()[0];

                        if(queryType != typeof(Object))
                        {
                            subManager.AddSubscription(queryType, interfaceType);
                        }
                    }
                }
                return subManager;
            });
        }


        private static void AddHandlerAsService(IServiceCollection services, Type type, bool isQuery)
        {
            //Decorator Pattern by binding to handler interface instead of concrete type.
            object[] attributes = type.GetCustomAttributes(false);

            List<Type> pipeline;
            if(isQuery)
            {
                 pipeline = attributes
                .Select(x => ToDecorator(x))
                .Concat(new[] { typeof(ReadOnlyDecorator<,>) })
                .Concat(new[] { type })
                .Reverse()
                .ToList();
            }
            else
            {
               pipeline = attributes
              .Select(x => ToDecorator(x))
              .Concat(new[] { type })
              .Reverse()
              .ToList();
            }

            IEnumerable<Type> interfaceTypes = type.GetInterfaces().Where(y => IsHandlerInterface(y));

            foreach (var interfaceType in interfaceTypes)
            {
                Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);

                Type commandOrQueryType = interfaceType.GetGenericArguments()[0];
                if (commandOrQueryType == typeof(Object))
                {
                    services.TryAddTransient(type, factory);
                }
                else
                {
                    //Decorator pattern
                    services.AddTransient(interfaceType, factory);
                }
            }
        }

        private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            List<ConstructorInfo> ctors = pipeline
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

           var a=  typeof(ReadOnlyDecorator<dynamic, dynamic>);

            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

                    object[] parameters = GetParameters(parameterInfos, current, provider);

                    //Handler
                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
            {
                result[i] = GetParameter(parameterInfos[i], current, provider);
            }

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (IsHandlerInterface(parameterType))
                return current;

            object service = provider.GetService(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");
        }

        private static Type ToDecorator(object attribute)
        {
            Type type = attribute.GetType();

            if (type == typeof(DatabaseRetryAttribute))
                return typeof(DatabaseRetryDecorator<,>);

            if (type == typeof(AuditLogAttribute))
                return typeof(AuditLoggingDecorator<,>);

            // other attributes go here

            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            return IsCommandHandlerInterface(type)  || IsQueryHandlerInterface(type);
        }

        private static bool IsCommandHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<,>);
        }

        private static bool IsQueryHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IQueryHandler<,>);
        }
    }
}
