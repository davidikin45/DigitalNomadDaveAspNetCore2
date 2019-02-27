using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IRunOnError
	{
        Task ExecuteAsync();
    }
}