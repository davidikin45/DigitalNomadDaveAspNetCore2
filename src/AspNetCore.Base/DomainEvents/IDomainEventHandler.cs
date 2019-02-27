using AspNetCore.Base.Validation;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        Task<Result> HandlePreCommitAsync(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }

    public interface IDynamicDomainEventHandler
    {
        Task<Result> HandlePreCommitAsync(dynamic domainEvent);
        Task<Result> HandlePostCommitAsync(dynamic domainEvent);
    }
}