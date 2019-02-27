using AspNetCore.Base.Cqrs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.DomainEvents.Subscriptions
{
    public partial class CqrsInMemoryCommandSubscriptionsManager : ICqrsCommandSubscriptionsManager
    {
        private readonly Dictionary<string, List<CommandSubscriptionInfo>> _handlers;
        private readonly List<Type> _commandTypes;

        public event EventHandler<string> OnCommandRemoved;

        public CqrsInMemoryCommandSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<CommandSubscriptionInfo>>();
            _commandTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription(Type commandType, Type commandHandlerType)
        {
            var commandName = GetCommandKey(commandType);

            DoAddSubscription(commandHandlerType, commandName);

            if (!_commandTypes.Contains(commandType))
            {
                _commandTypes.Add(commandType);
            }
        }

        public void AddSubscription<C, CH>()
            where C : ICommand
            where CH : ICommandHandler<C>
        {
            var commandName = GetCommandKey<C>();

            DoAddSubscription(typeof(CH), commandName);

            if (!_commandTypes.Contains(typeof(C)))
            {
                _commandTypes.Add(typeof(C));
            }
        }

        public void AddSubscription<C, R, CH>()
            where C : ICommand<R>
            where CH : ICommandHandler<C, R>
        {
            var commandName = GetCommandKey<C>();

            DoAddSubscription(typeof(CH), commandName);

            if (!_commandTypes.Contains(typeof(C)))
            {
                _commandTypes.Add(typeof(C));
            }
        }

        private void DoAddSubscription(Type handlerType, string commandName)
        {
            if (!HasSubscriptionsForCommand(commandName))
            {
                _handlers.Add(commandName, new List<CommandSubscriptionInfo>());
            }

            if (_handlers[commandName].Any())
            {
                throw new ArgumentException($"Handler already registered for '{commandName}'");
            }

            _handlers[commandName].Add(CommandSubscriptionInfo.Typed(handlerType));
        }

        public void RemoveSubscription<C, CH>()
            where C : ICommand
            where CH : ICommandHandler<C>
        {
            var handlerToRemove = FindSubscriptionToRemove<C, CH>();
            var commandName = GetCommandKey<C>();
            DoRemoveHandler(commandName, handlerToRemove);
        }

        public void RemoveSubscription<C, R, CH>()
           where C : ICommand<R>
           where CH : ICommandHandler<C, R>
        {
            var handlerToRemove = FindSubscriptionToRemove<C, R, CH>();
            var commandName = GetCommandKey<C>();
            DoRemoveHandler(commandName, handlerToRemove);
        }

        private void DoRemoveHandler(string commandName, CommandSubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[commandName].Remove(subsToRemove);
                if (!_handlers[commandName].Any())
                {
                    _handlers.Remove(commandName);
                    var commandType = _commandTypes.SingleOrDefault(e => e.Name == commandName);
                    if (commandType != null)
                    {
                        _commandTypes.Remove(commandType);
                    }
                    RaiseOnCommandRemoved(commandName);
                }

            }
        }

        public IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand(ICommand command)
        {
            var key = GetCommandKey(command.GetType());
            return GetSubscriptionsForCommand(key);
        }

        public IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand<C>() where C : ICommand
        {
            var key = GetCommandKey<C>();
            return GetSubscriptionsForCommand(key);
        }
        public IEnumerable<CommandSubscriptionInfo> GetSubscriptionsForCommand(string commandName) => _handlers[commandName];

        private void RaiseOnCommandRemoved(string commandName)
        {
            var handler = OnCommandRemoved;
            if (handler != null)
            {
                OnCommandRemoved(this, commandName);
            }
        }

        private CommandSubscriptionInfo FindSubscriptionToRemove<C, CH>()
             where C : ICommand
             where CH : ICommandHandler<C>
        {
            var commandName = GetCommandKey<C>();
            return DoFindSubscriptionToRemove(commandName, typeof(CH));
        }

        private CommandSubscriptionInfo FindSubscriptionToRemove<C, R, CH>()
             where C : ICommand<R>
             where CH : ICommandHandler<C, R>
        {
            var commandName = GetCommandKey<C>();
            return DoFindSubscriptionToRemove(commandName, typeof(CH));
        }

        private CommandSubscriptionInfo DoFindSubscriptionToRemove(string commandName, Type handlerType)
        {
            if (!HasSubscriptionsForCommand(commandName))
            {
                return null;
            }

            return _handlers[commandName].SingleOrDefault(s => s.HandlerType == handlerType);

        }

        public bool HasSubscriptionsForCommand<C>() where C : ICommand
        {
            var key = GetCommandKey<C>();
            return HasSubscriptionsForCommand(key);
        }

        public bool HasSubscriptionsForCommand(string commandName) => _handlers.ContainsKey(commandName);

        public Type GetCommandTypeByName(string commandName) => _commandTypes.SingleOrDefault(t => t.Name == commandName);

        public string GetCommandKey<C>()
        {
            return GetCommandKey(typeof(C));
        }

        private string GetCommandKey(Type commandType)
        {
            return commandType.Name;
        }
    }
}
