using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
    {
        Task<Result> HandleAsync(string eventName, TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }

    public interface ITypedIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
     where TIntegrationEvent : IntegrationEvent
    {

    }

    public interface IDynamicIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    {

    }
}
