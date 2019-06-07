using AspNetCore.Base.Cqrs;
using System;
using System.Collections.Generic;
using static AspNetCore.Base.DomainEvents.Subscriptions.CqrsInMemoryCommandSubscriptionsManager;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public interface ICqrsCommandSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnCommandRemoved;

        void AddDynamicSubscription<C, R, CH>(string commandName)
            where CH: IDynamicCommandHandler<C, R>;

        void AddDynamicSubscription<C, CH>(string commandName)
         where CH : IDynamicCommandHandler<C>;

        void RemoveDynamicSubscription<C, R, CH>(string eventName)
         where CH : IDynamicCommandHandler<C, R>;

        void AddSubscription(Type commandType, Type commandHandlerType);

        void AddSubscription<C, R, CH>()
          where C : ICommand<R>
          where CH : ITypedCommandHandler<C, R>;

        void AddSubscription<C, CH>()
          where C : ICommand
          where CH : ITypedCommandHandler<C>;

        void RemoveSubscription<C, R, CH>()
             where C : ICommand<R>
             where CH : ITypedCommandHandler<C, R>;

        bool HasSubscriptionsForCommand<C, R>() where C : ICommand<R>;
        bool HasSubscriptionsForCommand(string commandName);
        Type GetCommandTypeByName(string commandName);
        void Clear();

        IReadOnlyDictionary<string, CommandSubscriptionInfo> GetSubscriptions();
        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand<R>(ICommand<R> command);
        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand<C, R>() where C : ICommand<R>;
        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand(string commandName);
        IEnumerable<string> GetCommands();

        string GetCommandKey<T>();
    }
}
