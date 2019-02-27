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
            List<Type> commandHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsCommandHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type commandHandlerType in commandHandlerTypes)
            {
                AddHandlerAsService(services, commandHandlerType);
            }

            services.AddSingleton<ICqrsCommandSubscriptionsManager>(sp => {
                var subManager = new CqrsInMemoryCommandSubscriptionsManager();
                foreach (Type commandHandlerType in commandHandlerTypes)
                {
                    IEnumerable<Type> interfaceTypes = commandHandlerType.GetInterfaces().Where(y => IsHandlerInterface(y));

                    foreach (Type interfaceType in interfaceTypes)
                    {
                        Type commandType = interfaceType.GetGenericArguments()[0];

                        subManager.AddSubscription(commandType, commandHandlerType);
                    }
                }
                return subManager;
            });

            List<Type> queryHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsQueryHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type queryHandlerType in queryHandlerTypes)
            {
                AddHandlerAsService(services, queryHandlerType);
            }

            services.AddSingleton<ICqrsQuerySubscriptionsManager>(sp => {
                var subManager = new CqrsInMemoryQuerySubscriptionsManager();
                foreach (Type queryHandlerType in queryHandlerTypes)
                {
                    IEnumerable<Type> interfaceTypes = queryHandlerType.GetInterfaces().Where(y => IsHandlerInterface(y));

                    foreach (var interfaceType in interfaceTypes)
                    {
                        Type queryType = interfaceType.GetGenericArguments()[0];

                        subManager.AddSubscription(queryType, queryHandlerType);
                    }
                }
                return subManager;
            });
        }

        private static void AddHandlerAsService(IServiceCollection services, Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);

            List<Type> pipeline = attributes
                .Select(x => ToDecorator(x))
                .Concat(new[] { type })
                .Reverse()
                .ToList();

            IEnumerable<Type> interfaceTypes = type.GetInterfaces().Where(y => IsHandlerInterface(y));

            foreach (var interfaceType in interfaceTypes)
            {
                Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);
                services.AddTransient(interfaceType, factory);
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

            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

                    object[] parameters = GetParameters(parameterInfos, current, provider);

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
                return typeof(DatabaseRetryDecorator<>);

            if (type == typeof(AuditLogAttribute))
                return typeof(AuditLoggingDecorator<>);

            // other attributes go here

            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            return IsCommandHandlerInterface(type) || IsQueryHandlerInterface(type);
        }

        private static bool IsCommandHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();
  
            return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(ICommandHandler<,>);
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
