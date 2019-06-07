using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents.HandlersDynamic
{
    public abstract class DynamicIntegrationEventHandler<TIntegrationEvent> : IDynamicIntegrationEventHandler<TIntegrationEvent>
    {
        public Task<Result> HandleAsync(string eventName, TIntegrationEvent integrationEvent, CancellationToken cancellationToken)
            => Task.FromResult(Handle(eventName, integrationEvent));

        protected abstract Result Handle(string eventName, TIntegrationEvent integrationEvent);
    }

    public abstract class DynamicRequestIntegrationEventHandler<TDomainEvent> : DynamicIntegrationEventHandler<dynamic>
    {

    }
}
