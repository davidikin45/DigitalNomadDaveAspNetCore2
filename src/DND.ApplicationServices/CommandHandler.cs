using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Base.Cqrs.HandlersDynamic;
using AspNetCore.Base.Validation;
using DND.Core;
using DND.Domain.Blog.Authors;

namespace DND.ApplicationServices
{
    public class CommandHandler : AsyncDynamicRequestResponseCommandHandler
    {
        private readonly IAppUnitOfWork _appUnitOfWork;

        public CommandHandler(IAppUnitOfWork appUnitOfWork)
        {
            _appUnitOfWork = appUnitOfWork;
        }


        public async override Task<Result<object>> HandleAsync(string commandName, dynamic command, CancellationToken cancellationToken = default)
        {
            _appUnitOfWork.AuthorRepository.Add(new Author() { Name="David Ikin"}, "");

            await _appUnitOfWork.CompleteAsync();

            return Result.Ok<object>(null);
        }
    }
}
