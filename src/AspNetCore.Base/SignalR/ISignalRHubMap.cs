using Microsoft.AspNetCore.SignalR;

namespace AspNetCore.Base.SignalR
{
    public interface ISignalRHubMap
    {
        void MapHub(HubRouteBuilder routes, string hubPathPrefix);
    }
}
