using AspNetCore.Base.DomainEvents.Subscriptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public interface IDomainEventBus
    {
        IDomainEventBusSubscriptionsManager DomainEventSubscriptionsManager { get; }

        Task PublishPreCommitAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);

        Task PublishPostCommitAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task PublishPostCommitBatchAsync(IEnumerable<DomainEvent> domainEvent, CancellationToken cancellationToken = default);

        Task ProcessPostCommitAsync(string eventName, string payload);

        Task ProcessPostCommitHandlerAsync(string eventName, string payload, string handlerType, int handlerIndex);
    }
}