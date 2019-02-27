using AspNetCore.Base.SignalR;
using DND.ApplicationServices.Blog.Categories.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Category.Notifications
{
    public class CategoriesNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<CategoryDto>>(signalRUrlPrefix + "/blog/categories/notifications");
        }
    }
}
