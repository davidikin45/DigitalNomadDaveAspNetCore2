using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IAsyncInitializer
    {
        Task ExecuteAsync();
    }
}