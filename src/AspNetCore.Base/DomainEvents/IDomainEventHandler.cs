using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.DomainEvents
{
    public interface IDomainEventHandler<in T>
    {
        //Trigger Transactional Effects
        Task<Result> HandlePreCommitAsync(string eventName, T domainEvent, CancellationToken cancellationToken = default);

        //Trigger Integration Events
        Task<Result> HandlePostCommitAsync(string eventName, T domainEvent, CancellationToken cancellationToken = default);
    }

    public interface ITypedDomainEventHandler<in T> : IDomainEventHandler<T>
        where T : DomainEvent
    {

    }

    public interface IDynamicDomainEventHandler<in T> : IDomainEventHandler<T>
    {

    }
}