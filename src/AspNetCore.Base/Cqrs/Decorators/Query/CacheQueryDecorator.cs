using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class CacheQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public CacheQueryDecorator(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public async Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken = default)
        {
            TResult result = await _handler.HandleAsync(queryName, query, cancellationToken);
            return result;
        }
    }
}
