using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IRunOnEachRequest
	{
		Task ExecuteAsync();
	}
}