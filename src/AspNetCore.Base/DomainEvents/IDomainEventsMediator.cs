using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public interface IDomainEventsMediator
    {
        Task DispatchPreCommitAsync(IDomainEvent domainEvent);

        Task DispatchPostCommitAsync(IDomainEvent domainEvent);
        Task DispatchPostCommitBatchAsync(IEnumerable<IDomainEvent> domainEvent);

        Task HandlePostCommitDispatchAsync(DomainEventMessage message);
        Task HandlePostCommitAsync(DomainEventHandlerMessage messaget);
    }
}