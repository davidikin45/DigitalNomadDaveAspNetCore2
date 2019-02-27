using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class CacheQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public CacheQueryDecorator(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            TResult result = await _handler.HandleAsync(query);
            return result;
        }
    }
}
