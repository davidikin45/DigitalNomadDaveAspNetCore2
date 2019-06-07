using System;
using System.Collections.Generic;
using static AspNetCore.Base.IntegrationEvents.Subscriptions.IntegrationEventBusInMemorySubscriptionsManager;

namespace AspNetCore.Base.IntegrationEvents.Subscriptions
{
    public interface IIntegrationEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        void AddDynamicSubscription<TIntegrationEvent,TH>(string eventType)
         where TH : IDynamicIntegrationEventHandler<TIntegrationEvent>;

        void RemoveDynamicSubscription<TIntegrationEvent,TH>(string eventType)
            where TH : IDynamicIntegrationEventHandler<TIntegrationEvent>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventType);
        Type GetEventTypeByName(string eventType);
        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(IntegrationEvent @event);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();
        string GetEventKey(Type integrationEventType);
    }
}
