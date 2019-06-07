using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.HandlersTyped
{
    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    {
        Task<TResult> IQueryHandler<TQuery, TResult>.HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken)
          => Task.FromResult(Handle(queryName, query));

        protected abstract TResult Handle(string queryName, TQuery query);
    }
}
