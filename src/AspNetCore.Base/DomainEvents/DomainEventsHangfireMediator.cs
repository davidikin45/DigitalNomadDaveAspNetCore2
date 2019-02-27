using AspNetCore.Base.DomainEvents.Subscriptions;
using AspNetCore.Base.Settings;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEventsHangfireMediator : DomainEventsInMemoryMediator
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ServerSettings _serverSettings;

        public DomainEventsHangfireMediator(
            IServiceProvider serviceProvider, 
            IBackgroundJobClient backgroundJobClient, 
            IDomainEventSubscriptionsManager domainEventSubscriptionsManager, 
            ServerSettings serverSettings)
            :base(serviceProvider, domainEventSubscriptionsManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _serverSettings = serverSettings;
        }

        #region Dispatch Post Commit Integration Events
        public override Task DispatchPostCommitAsync(IDomainEvent domainEvent)
        {
            var domainEventMessage = new DomainEventMessage(_serverSettings.ServerName, domainEvent);

            try
            {
                var job = Job.FromExpression<IDomainEventsMediator>(m => m.HandlePostCommitDispatchAsync(domainEventMessage));
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

        #region Handle Post Commit Integration Events - Handled out of process in HangFire
        protected override Task TryDispatchHandlerPostCommitAsync(DomainEventHandlerMessage domainEventHandlerMessage)
        {
            try
            {
                //Each Post Commit Domain Event Handling is completely independent. By registering the event AND handler (rather than just the event) in hangfire we get the granularity of retrying at a event/handler level.
                //Hangfire unfortunately uses System.Type.GetType to get job type. This only looks at the referenced assemblies of the web project and not the dynamic loaded plugins so need to
                //proxy back through this common assembly.

                var job = Job.FromExpression<IDomainEventsMediator>(m => m.HandlePostCommitAsync(domainEventHandlerMessage));

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
