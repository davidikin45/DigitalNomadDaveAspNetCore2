using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.HostedServices
{
    public interface IHostedServiceCronJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
