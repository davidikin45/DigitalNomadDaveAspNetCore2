using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IRunAfterEachRequest
	{
        Task ExecuteAsync();
    }
}