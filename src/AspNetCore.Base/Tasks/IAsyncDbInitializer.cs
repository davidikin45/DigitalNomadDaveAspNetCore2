using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IAsyncDbInitializer
    {
        Task ExecuteAsync();
        int Order { get; }
    }
}