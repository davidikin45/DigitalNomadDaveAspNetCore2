using AspNetCore.Base.SignalR;
using DND.ApplicationServices.Blog.Locations.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Locations.Notifications
{
    public class LocationsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<LocationDto>>(signalRUrlPrefix + "/blog/locations/notifications");
        }
    }
}
