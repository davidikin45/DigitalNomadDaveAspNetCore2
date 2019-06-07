using AspNetCore.Base.DomainEvents.Subscriptions;
using AspNetCore.Base.Settings;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEventBusHangFire : DomainEventBusInMemory
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ServerSettings _serverSettings;

        public DomainEventBusHangFire(
            IServiceProvider serviceProvider, 
            IBackgroundJobClient backgroundJobClient, 
            IDomainEventBusSubscriptionsManager domainEventSubscriptionsManager, 
            ServerSettings serverSettings)
            :base(serviceProvider, domainEventSubscriptionsManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _serverSettings = serverSettings;
        }

        #region Publish Post Commit Integration Events
        public override Task PublishPostCommitAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerTypes = DomainEventSubscriptionsManager.GetHandlersForEvent(domainEvent).ToList();

            //Only persist domainEvent to Hangfire if it has > 0 handlers.
            if(handlerTypes.Count > 0)
            {

                var eventName = DomainEventSubscriptionsManager.GetEventKey(domainEvent.GetType());
                var payload = JsonConvert.SerializeObject(domainEvent);

                try
                {
                    var job = Job.FromExpression<IDomainEventBus>(m => m.ProcessPostCommitAsync(eventName, payload));
                    var queue = new EnqueuedState(_serverSettings.ServerName);
                    _backgroundJobClient.Create(job, queue);
                }
                catch
                {
                    //Log Hangfire Post commit event Background enqueue failed
                }
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Handle Post Commit Domain Events - Handled out of process in HangFire
        protected override Task TryProcessPostCommitHandlerAsync(string eventName, string payload, string handlerType, int handlerIndex)
        {
            try
            {
                //Each Post Commit Domain Event Handling is completely independent. By registering the event AND handler (rather than just the event) in hangfire we get the granularity of retrying at a event/handler level.
                //Hangfire unfortunately uses System.Type.GetType to get job type. This only looks at the referenced assemblies of the web project and not the dynamic loaded plugins so need to
                //proxy back through this common assembly.

                var job = Job.FromExpression<IDomainEventBus>(m => m.ProcessPostCommitHandlerAsync(eventName, payload, handlerType, handlerIndex));

                var queue = new EnqueuedState(_serverSettings.ServerName);
                _backgroundJobClient.Create(job, queue);
            }
            catch
            {
                //Log Hangfire Post commit event Background enqueue failed
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
