using AspNetCore.Base.IntegrationEvents.Subscriptions;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.Base.IntegrationEvents
{
    public static class IntegrationEventsServiceCollectionExtensions
    {
        public static void AddIntegrationEvents(this IServiceCollection services, string hostName, string userName, string password, string subscriptionClientName, int retryCount = 5)
        {
            services.AddRabbitMQPersistentConnection(hostName, userName, password, retryCount);
            services.AddEventBus(subscriptionClientName, retryCount);
            services.AddIntegrationEventHandlers(new List<Assembly>() { Assembly.GetCallingAssembly() });
        }

        public static void AddIntegrationEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies, string hostName, string userName, string password, string subscriptionClientName, int retryCount = 5)
        {
            services.AddRabbitMQPersistentConnection(hostName, userName, password, retryCount);
            services.AddEventBus(subscriptionClientName, retryCount);
            services.AddIntegrationEventHandlers(assemblies);
        }

        public static void AddRabbitMQPersistentConnection(this IServiceCollection services, string hostName, string userName, string password, int retryCount)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = hostName
                };

                if (!string.IsNullOrEmpty(userName))
                {
                    factory.UserName = userName;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    factory.Password = password;
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
        }

        public static void AddEventBus(this IServiceCollection services, string subscriptionClientName, int retryCount)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
        }

        public static void AddIntegrationEventHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            List<Type> integrationEventHandlerTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type integrationEventHandlerType in integrationEventHandlerTypes)
            {
                AddHandlerAsService(services, integrationEventHandlerType);
            }

            services.AddSingleton<IEventBusSubscriptionsManager>(sp => {
                var subManager = new InMemoryEventBusSubscriptionsManager();
                foreach (Type integrationEventHandlerType in integrationEventHandlerTypes.Where(x => x.GetInterfaces().Any(y => IsIntegrationEventHandlerInterface(y))))
                {
                    IEnumerable<Type> interfaceTypes = integrationEventHandlerType.GetInterfaces().Where(y => IsHandlerInterface(y));
                    foreach (var interfaceType in interfaceTypes)
                    {
                        Type eventType = interfaceType.GetGenericArguments()[0];

                        subManager.AddSubscription(eventType, integrationEventHandlerType);
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


            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            return IsIntegrationEventHandlerInterface(type) || IsDynamicIntegrationEventHandlerInterface(type);
        }

        private static bool IsIntegrationEventHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IIntegrationEventHandler<>);
        }

        private static bool IsDynamicIntegrationEventHandlerInterface(Type type)
        {
            return type == typeof(IDynamicIntegrationEventHandler);
        }

        //private void ConfigureEventBus(IApplicationBuilder app)
        //{
        //    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        //    eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
        //}
    }
}
