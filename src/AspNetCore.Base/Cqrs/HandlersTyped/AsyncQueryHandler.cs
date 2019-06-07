using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.HandlersTyped
{
    public abstract class AsyncQueryHandler<TQuery, TResult> : ITypedQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public abstract Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken = default);
    }
}
