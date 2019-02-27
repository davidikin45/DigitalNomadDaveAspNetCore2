using AspNetCore.Base.Validation;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public interface ICqrsMediator
    {
        Task<Result> DispatchAsync(ICommand command);
        Task<Result<T>> DispatchAsync<T>(ICommand<T> command);

        Task<T> DispatchAsync<T>(IQuery<T> query);
    }
}
