using System;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public partial class CqrsInMemoryCommandSubscriptionsManager : ICqrsCommandSubscriptionsManager
    {
        public class CommandSubscriptionInfo
        {
            public bool IsDynamic { get; }
            public string CommandName { get; }
            public Type CommandType { get; }
            public Type ReturnType { get; }
            public Type HandlerType { get; }

            private CommandSubscriptionInfo(bool isDynamic, string commandName, Type commandType, Type returnType, Type handlerType)
            {
                IsDynamic = isDynamic;
                CommandName = commandName;
                CommandType = commandType;
                ReturnType = returnType;
                HandlerType = handlerType;
            }

            public static CommandSubscriptionInfo Typed(string commandName, Type commandType, Type returnType, Type handlerType)
            {
                return new CommandSubscriptionInfo(false, commandName, commandType, returnType, handlerType);
            }

            public static CommandSubscriptionInfo Dynamic(string commandName, Type returnType, Type handlerType)
            {
                return new CommandSubscriptionInfo(false, commandName, null, returnType, handlerType);
            }
        }
    }
}
