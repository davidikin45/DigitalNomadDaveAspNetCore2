using AspNetCore.Base.Cqrs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public partial class CqrsInMemoryQuerySubscriptionsManager : ICqrsQuerySubscriptionsManager
    {
        private readonly Dictionary<string, List<QuerySubscriptionInfo>> _handlers;
        private readonly List<Type> _queryTypes;

        public event EventHandler<string> OnQueryRemoved;

        public CqrsInMemoryQuerySubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<QuerySubscriptionInfo>>();
            _queryTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription(Type queryType, Type queryHandlerType)
        {
            var queryName = GetQueryKey(queryType);

            DoAddSubscription(queryHandlerType, queryName);

            if (!_queryTypes.Contains(queryType))
            {
                _queryTypes.Add(queryType);
            }
        }

        public void AddSubscription<Q, R, QH>()
            where Q : IQuery<R>
            where QH : IQueryHandler<Q, R>
        {
            var queryName = GetQueryKey<Q>();

            DoAddSubscription(typeof(QH), queryName);

            if (!_queryTypes.Contains(typeof(Q)))
            {
                _queryTypes.Add(typeof(Q));
            }
        }

        private void DoAddSubscription(Type handlerType, string queryName)
        {
            if (!HasSubscriptionsForQuery(queryName))
            {
                _handlers.Add(queryName, new List<QuerySubscriptionInfo>());
            }

            if (_handlers[queryName].Any())
            {
                throw new ArgumentException($"Handler Type already registered for '{queryName}'");
            }

            _handlers[queryName].Add(QuerySubscriptionInfo.Typed(handlerType));
        }

        public void RemoveSubscription<Q, R, QH>()
           where Q : IQuery<R>
           where QH : IQueryHandler<Q, R>
        {
            var handlerToRemove = FindSubscriptionToRemove<Q, R, QH>();
            var queryName = GetQueryKey<Q>();
            DoRemoveHandler(queryName, handlerToRemove);
        }

        private void DoRemoveHandler(string queryName, QuerySubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[queryName].Remove(subsToRemove);
                if (!_handlers[queryName].Any())
                {
                    _handlers.Remove(queryName);
                    var queryType = _queryTypes.SingleOrDefault(e => e.Name == queryName);
                    if (queryType != null)
                    {
                        _queryTypes.Remove(queryType);
                    }
                    RaiseOnQueryRemoved(queryName);
                }

            }
        }

        public IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<TResult>(IQuery<TResult> query)
        {
            var key = GetQueryKey(query.GetType());
            return GetSubscriptionsForQuery(key);
        }

        public IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery<Q, TResult>() where Q : IQuery<TResult>
        {
            var key = GetQueryKey<Q>();
            return GetSubscriptionsForQuery(key);
        }

        public IEnumerable<QuerySubscriptionInfo> GetSubscriptionsForQuery(string queryName) => _handlers[queryName];

        private void RaiseOnQueryRemoved(string queryName)
        {
            var handler = OnQueryRemoved;
            if (handler != null)
            {
                OnQueryRemoved(this, queryName);
            }
        }

        private QuerySubscriptionInfo FindSubscriptionToRemove<Q, R, QH>()
             where Q : IQuery<R>
             where QH : IQueryHandler<Q, R>
        {
            var queryName = GetQueryKey<Q>();
            return DoFindSubscriptionToRemove(queryName, typeof(QH));
        }

        private QuerySubscriptionInfo DoFindSubscriptionToRemove(string queryName, Type handlerType)
        {
            if (!HasSubscriptionsForQuery(queryName))
            {
                return null;
            }

            return _handlers[queryName].SingleOrDefault(s => s.HandlerType == handlerType);

        }

        public bool HasSubscriptionsForQuery<Q, R>() where Q : IQuery<R>
        {
            var key = GetQueryKey<Q>();
            return HasSubscriptionsForQuery(key);
        }

        public bool HasSubscriptionsForQuery(string queryName) => _handlers.ContainsKey(queryName);

        public Type GetQueryTypeByName(string queryName) => _queryTypes.SingleOrDefault(t => t.Name == queryName);

        public string GetQueryKey<Q>()
        {
            return GetQueryKey(typeof(Q));
        }

        private string GetQueryKey(Type queryType)
        {
            return queryType.Name;
        }
    }
}
