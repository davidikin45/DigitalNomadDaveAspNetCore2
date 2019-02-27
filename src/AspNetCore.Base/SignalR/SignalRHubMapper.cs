using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.SignalR
{
    public class SignalRHubMapper : ISignalRHubMapper
    {
        private IEnumerable<ISignalRHubMap> Maps;
        public SignalRHubMapper(IEnumerable<ISignalRHubMap> maps)
        {
            Maps = maps;
        }

        public void MapHubs(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            foreach (var map in Maps)
            {
                map.MapHub(routes, signalRUrlPrefix);
            }
        }
    }

    public interface ISignalRHubMapper
    {
        void MapHubs(HubRouteBuilder routes, string signalRUrlPrefix);
    }
}
