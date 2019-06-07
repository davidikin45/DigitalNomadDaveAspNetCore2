using System;
using System.Collections.Generic;
using static AspNetCore.Base.DomainEvents.Subscriptions.DomainEventBusInMemorySubscriptionsManager;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public interface IDomainEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
           where T : DomainEvent
           where TH : IDomainEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IDomainEventHandler<T>
             where T : DomainEvent;

        void RemoveDynamicSubscription<TDomainEvent,TH>(string eventName)
            where TH : IDynamicDomainEventHandler<TDomainEvent>;

        void AddDynamicSubscription<TDomainEvent,TH>(string eventName)
            where TH : IDynamicDomainEventHandler<TDomainEvent>;

        bool HasSubscriptionsForEvent<T>() where T : DomainEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(DomainEvent domainEvent);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : DomainEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey(Type domainEventType);
        string GetEventKey<T>();
    }
}
