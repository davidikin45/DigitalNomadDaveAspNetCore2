using System;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public partial class CqrsInMemoryQuerySubscriptionsManager : ICqrsQuerySubscriptionsManager
    {
        public class QuerySubscriptionInfo
        {
            public Type HandlerType { get; }

            private QuerySubscriptionInfo(Type handlerType)
            {
                HandlerType = handlerType;
            }

            public static QuerySubscriptionInfo Typed(Type handlerType)
            {
                return new QuerySubscriptionInfo(handlerType);
            }
        }
    }
}
