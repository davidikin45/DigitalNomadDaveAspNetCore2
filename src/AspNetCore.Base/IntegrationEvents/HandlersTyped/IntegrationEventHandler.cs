using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents.HandlersTyped
{
    public abstract class IntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
    {
        public Task<Result> HandleAsync(string eventName, TIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var result = Handle(eventName, integrationEvent);
            return Task.FromResult(result);
        }


        protected abstract Result Handle(string eventName, TIntegrationEvent integrationEvent);

    }
}
