using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
