using AspNetCore.Base.SignalR;
using DND.ApplicationServices.Blog.Tags.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Tag.Notifications
{
    public class TagsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<TagDto>>(signalRUrlPrefix + "/blog/tags/notifications");
        }
    }
}
