using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public interface IRunAfterApplicationConfiguration
    {
        void Execute();
    }
}