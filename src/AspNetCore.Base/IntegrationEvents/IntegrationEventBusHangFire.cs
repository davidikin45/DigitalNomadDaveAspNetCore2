using AspNetCore.Base.IntegrationEvents.Subscriptions;
using AspNetCore.Base.Settings;
using Autofac;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents
{
    //Each microservice has a seperate queue 
    //Publisher > Exchange > Queue > Consumer
    public class IntegrationEventBusHangFire : IntegrationEventBusInMemory
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ServerSettings _serverSettings;

        public IntegrationEventBusHangFire(IBackgroundJobClient backgroundJobClient, ILogger<IntegrationEventBusHangFire> logger,
            IServiceProvider serviceProvider, IIntegrationEventBusSubscriptionsManager subsManager, ServerSettings serverSettings)
            :base(logger, serviceProvider, subsManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _serverSettings = serverSettings;
        }

        public override Task PublishAsync(IntegrationEvent integrationEvent)
        {
            var eventName = _subsManager.GetEventKey(integrationEvent.GetType());
            var payload = JsonConvert.SerializeObject(integrationEvent);

            foreach (var serverName in _serverSettings.ServerNames)
            {
                var job = Job.FromExpression<IIntegrationEventBus>(m => m.ProcessEventAsync(eventName, payload));
                var queue = new EnqueuedState(serverName);
                _backgroundJobClient.Create(job, queue);
            }

            return Task.CompletedTask;
        }
    }
}
