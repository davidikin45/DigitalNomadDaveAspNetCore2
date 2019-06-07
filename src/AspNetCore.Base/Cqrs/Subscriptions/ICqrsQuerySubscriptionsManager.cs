using AspNetCore.Base.Cqrs;
using System;
using System.Collections.Generic;
using static AspNetCore.Base.DomainEvents.Subscriptions.CqrsInMemoryQuerySubscriptionsManager;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public interface ICqrsQuerySubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnQueryRemoved;

        void AddDynamicSubscription<Q, R, QH>(string queryName)
        where QH : IDynamicQueryHandler<Q, R>;

        void RemoveDynamicSubscription<Q, R, QH>(string eventName)
        where QH : IDynamicQueryHandler<Q, R>;

        void AddSubscription<Q, R, QH>()
           where Q : IQuery<R>
           where QH : ITypedQueryHandler<Q, R>;

        void RemoveSubscription<Q, R, QH>()
              where Q : IQuery<R>
             where QH : ITypedQueryHandler<Q, R>;

        bool HasSubscriptionsForQuery<Q, TResult>() where Q : IQuery<TResult>;
        bool HasSubscriptionsForQuery(string queryName);
        Type GetQueryTypeByName(string queryName);

        void Clear();

        IReadOnlyDictionary<string, QuerySubscriptionInfo> GetSubscriptions();
        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<TResult>(IQuery<TResult> query);
        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<Q, TResult>() where Q : IQuery<TResult>;
        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery(string queryName);
        IEnumerable<string> GetQueries();
        string GetQueryKey<T>();
    }
}
