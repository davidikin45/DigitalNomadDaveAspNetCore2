using AspNetCore.Base.SignalR;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Blog.Notifications
{
    public class BlogPostsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<BlogPostDto>>(signalRUrlPrefix + "/blog/blog-posts/notifications");
        }
    }
}
