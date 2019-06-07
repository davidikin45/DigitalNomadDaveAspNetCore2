using AspNetCore.Base.DomainEvents.HandlersDynamic;
using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class DomainEventHandler : AsyncDynamicRequestDomainEventHandler
    {
        public override Task<Result> HandlePostCommitAsync(string eventName, dynamic domainEvent, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Ok());
        }

        public override Task<Result> HandlePreCommitAsync(string eventName, dynamic domainEvent, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
