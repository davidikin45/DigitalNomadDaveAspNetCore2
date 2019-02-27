using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.CarouselItem.Notifications
{
    public class CarouselItemsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<CarouselItemDto>>(signalRUrlPrefix + "/cms/carousel-items/notifications");
        }
    }
}
