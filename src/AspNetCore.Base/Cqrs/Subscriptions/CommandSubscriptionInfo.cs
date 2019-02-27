using System;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public partial class CqrsInMemoryCommandSubscriptionsManager : ICqrsCommandSubscriptionsManager
    {
        public class CommandSubscriptionInfo
        {
            public Type HandlerType { get; }

            private CommandSubscriptionInfo(Type handlerType)
            {
                HandlerType = handlerType;
            }

            public static CommandSubscriptionInfo Typed(Type handlerType)
            {
                return new CommandSubscriptionInfo(handlerType);
            }
        }
    }
}
