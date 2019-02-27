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

        void AddSubscription<Q, R, QH>()
           where Q : IQuery<R>
           where QH : IQueryHandler<Q, R>;

        void RemoveSubscription<Q, R, QH>()
             where Q : IQuery<R>
             where QH : IQueryHandler<Q, R>;

        bool HasSubscriptionsForQuery<Q, TResult>() where Q : IQuery<TResult>;
        bool HasSubscriptionsForQuery(string queryName);
        Type GetQueryTypeByName(string queryName);

        void Clear();

        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<TResult>(IQuery<TResult> query);
        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<Q, TResult>() where Q : IQuery<TResult>;
        IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery(string queryName);

        string GetQueryKey<T>();
    }
}
