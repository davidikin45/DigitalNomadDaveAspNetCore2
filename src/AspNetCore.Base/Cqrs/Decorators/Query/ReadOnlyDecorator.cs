using AspNetCore.Base.Cqrs.HandlersDynamic;
using AspNetCore.Base.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class ReadOnlyDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    { 
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly IUnitOfWork[] _unitOfWorks;

        public ReadOnlyDecorator(IQueryHandler<TQuery, TResult> handler, IUnitOfWork[] unitOfWorks)
        {
            _handler = handler;
            _unitOfWorks = unitOfWorks;
        }

        public async Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWorks.ToList().ForEach(uow => uow.AutoDetectChangesEnabled = false);
                _unitOfWorks.ToList().ForEach(uow => uow.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking);

                TResult result = await _handler.HandleAsync(queryName, query, cancellationToken);
                return result;
            }
            finally
            {
                _unitOfWorks.ToList().ForEach(uow => uow.AutoDetectChangesEnabled = true);
                _unitOfWorks.ToList().ForEach(uow => uow.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll);
            }
        }
    }
}
