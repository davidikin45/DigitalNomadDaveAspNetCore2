using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.Faqs.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Faq.Notifications
{
    public class FaqsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<FaqDto>>(signalRUrlPrefix + "/cms/faqs/notifications");
        }
    }
}
