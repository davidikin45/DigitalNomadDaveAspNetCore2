using AspNetCore.Base.Cqrs.HandlersDynamic;
using DND.Core;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class QueryHandler : AsyncDynamicRequestResponseQueryHandler
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        public QueryHandler(IAppUnitOfWork appUnitOfWork)
        {
            _appUnitOfWork = appUnitOfWork;
        }

        public override async Task<object> HandleAsync(string queryName, dynamic query, CancellationToken cancellationToken = default)
        {
            var result = await _appUnitOfWork.AuthorRepository.GetAllAsync(default);

            return result;
        }

    }
}
