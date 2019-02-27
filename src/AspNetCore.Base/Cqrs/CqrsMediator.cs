using AspNetCore.Base.DomainEvents.Subscriptions;
using AspNetCore.Base.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public sealed class CqrsMediator : ICqrsMediator
    {
        private readonly IServiceProvider _provider;

        private readonly ICqrsCommandSubscriptionsManager _cqrsCommandSubscriptionManager;
        private readonly ICqrsQuerySubscriptionsManager _cqrsQuerySubscriptionManager;

        public CqrsMediator(IServiceProvider provider, ICqrsCommandSubscriptionsManager cqrsCommandSubscriptionManager, ICqrsQuerySubscriptionsManager cqrsQuerySubscriptionManager)
        {
            _provider = provider;
            _cqrsCommandSubscriptionManager = cqrsCommandSubscriptionManager;
            _cqrsQuerySubscriptionManager = cqrsQuerySubscriptionManager;
        }

        public async Task<Result> DispatchAsync(ICommand command)
        {
            Type handlerType = _cqrsCommandSubscriptionManager.GetSubscriptionsForCommand(command).First().HandlerType;

            dynamic handler = _provider.GetService(handlerType);
            Result result = await handler.HandleAsync((dynamic)command);
            return result;
        }

        public async Task<Result<T>> DispatchAsync<T>(ICommand<T> command)
        {
            Type handlerType = _cqrsCommandSubscriptionManager.GetSubscriptionsForCommand(command).First().HandlerType;

            dynamic handler = _provider.GetService(handlerType);
            Result<T> result = await handler.HandleAsync((dynamic)command);

            return result;
        }

        public async Task<T> DispatchAsync<T>(IQuery<T> query)
        {
            Type handlerType = _cqrsQuerySubscriptionManager.GetSubscriptionsForQuery(query).First().HandlerType;

            dynamic handler = _provider.GetService(handlerType);
            T result = await handler.HandleAsync((dynamic)query);

            return result;
        }
    }
}
