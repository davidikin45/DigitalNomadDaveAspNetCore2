using AspNetCore.Base.DomainEvents.Subscriptions;
using AspNetCore.Base.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEventsInMemoryMediator : IDomainEventsMediator
    {
        public static bool DispatchPostCommitEventsInParellel = true;

        protected readonly IServiceProvider _serviceProvider;
        protected readonly IDomainEventSubscriptionsManager _domainEventSubscriptionsManager;

        public DomainEventsInMemoryMediator(
            IServiceProvider serviceProvider, 
            IDomainEventSubscriptionsManager domainEventSubscriptionsManager)
        {
            _serviceProvider = serviceProvider;
            _domainEventSubscriptionsManager = domainEventSubscriptionsManager;
        }

        #region Dispatch Pre Commit InProcess Domain Events
        public async Task DispatchPreCommitAsync(IDomainEvent domainEvent)
        {
            var handlerTypes = _domainEventSubscriptionsManager.GetHandlersForEvent(domainEvent);
            foreach (var handlerType in handlerTypes)
            {
                dynamic handler = _serviceProvider.GetService(handlerType.HandlerType);
                Result result = await handler.HandlePreCommitAsync((dynamic)domainEvent);
                if (result.IsFailure)
                {
                    throw new Exception("Pre Commit Event Failed");
                }
            }
        }
        #endregion

        #region Dispatch Post Commit Integration Events
        public async Task DispatchPostCommitBatchAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            if (DispatchPostCommitEventsInParellel)
            {
                await Task.Run(() => Parallel.ForEach(domainEvents, async domainEvent =>
                {
                    await DispatchPostCommitAsync(domainEvent);
                }));
            }
            else
            {
                foreach (var domainEvent in domainEvents)
                {
                    await DispatchPostCommitAsync(domainEvent);
                }
            }
        }

        public virtual async Task DispatchPostCommitAsync(IDomainEvent domainEvent)
        {
            var domainEventMessage = new DomainEventMessage("", domainEvent);

            try
            {
                await HandlePostCommitDispatchAsync(domainEventMessage).ConfigureAwait(false);
            }
            catch
            {
                //Log InProcess Post commit event failed
            }
        }
        #endregion

        #region Handle Post Commit Events
        //Event Dispatcher
        public async Task HandlePostCommitDispatchAsync(DomainEventMessage domainEventMessage)
        {
            var handlerTypes = _domainEventSubscriptionsManager.GetHandlersForEvent(domainEventMessage.DomainEvent);

            if (DispatchPostCommitEventsInParellel)
            {
                await Task.Run(() => Parallel.ForEach(handlerTypes, async handlerType =>
                {
                    var domainEventHandlerMessage = new DomainEventHandlerMessage(handlerType.HandlerType.FullName, domainEventMessage);
                    await TryDispatchHandlerPostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
                }));
            }
            else
            {
                foreach (var handlerType in handlerTypes)
                {
                    var domainEventHandlerMessage = new DomainEventHandlerMessage(handlerType.HandlerType.FullName, domainEventMessage);
                    await TryDispatchHandlerPostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
                }
            }
        }

        protected virtual async Task TryDispatchHandlerPostCommitAsync(DomainEventHandlerMessage domainEventHandlerMessage)
        {
            try
            {
                await HandlePostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
            }
            catch
            {
                //Log InProcess Post commit event failed
            }
        }

        public async Task HandlePostCommitAsync(DomainEventHandlerMessage domainEventHandlerMessage)
        {
            Type handlerType = System.Type.GetType(domainEventHandlerMessage.HandlerType);
            if (handlerType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    handlerType = assembly.GetType(domainEventHandlerMessage.HandlerType);
                    if (handlerType != null)
                    {
                        break;
                    }
                }
            }

            if (handlerType == null)
            {
                throw new Exception("Invalid handler type");
            }

            await DispatchPostCommitAsync(handlerType, domainEventHandlerMessage.DomainEventMessage.DomainEvent).ConfigureAwait(false);
        }

        private async Task DispatchPostCommitAsync(Type handlerType, IDomainEvent domainEvent)
        {
            dynamic handler = _serviceProvider.GetService(handlerType);
            Result result = await handler.HandlePostCommitAsync((dynamic)domainEvent);
            if (result.IsFailure)
            {
                throw new Exception("Post Commit Event Failed");
            }
        }
        #endregion
    }
}
