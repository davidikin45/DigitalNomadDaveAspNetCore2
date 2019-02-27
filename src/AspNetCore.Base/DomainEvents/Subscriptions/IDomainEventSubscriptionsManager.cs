using System;
using System.Collections.Generic;
using static AspNetCore.Base.DomainEvents.Subscriptions.InMemoryDomainEventSubscriptionsManager;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public interface IDomainEventSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
           where T : IDomainEvent
           where TH : IDomainEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IDomainEventHandler<T>
             where T : IDomainEvent;

        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicDomainEventHandler;

        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicDomainEventHandler;

        bool HasSubscriptionsForEvent<T>() where T : IDomainEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(IDomainEvent @event);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IDomainEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey(Type domainEventType);
        string GetEventKey<T>();
    }
}
