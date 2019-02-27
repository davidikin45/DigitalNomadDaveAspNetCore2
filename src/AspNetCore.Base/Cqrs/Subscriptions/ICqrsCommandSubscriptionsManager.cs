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

        void AddSubscription(Type commandType, Type commandHandlerType);

        void AddSubscription<C, CH>()
           where C : ICommand
           where CH : ICommandHandler<C>;

        void RemoveSubscription<C, CH>()
              where C : ICommand
             where CH : ICommandHandler<C>;

        void AddSubscription<C, R, CH>()
          where C : ICommand<R>
          where CH : ICommandHandler<C, R>;

        void RemoveSubscription<C, R, CH>()
             where C : ICommand<R>
             where CH : ICommandHandler<C, R>;

        bool HasSubscriptionsForCommand<T>() where T : ICommand;
        bool HasSubscriptionsForCommand(string commandName);
        Type GetCommandTypeByName(string commandName);
        void Clear();

        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand(ICommand command);
        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand<T>() where T : ICommand;
        IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand(string commandName);

        string GetCommandKey<T>();
    }
}
