using AspNetCore.Base.SignalR;
using DND.ApplicationServices.Blog.Authors.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Author.Notifications
{
    public class AuthorsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<AuthorDto>>(signalRUrlPrefix + "/blog/authors/notifications");
        }
    }
}
