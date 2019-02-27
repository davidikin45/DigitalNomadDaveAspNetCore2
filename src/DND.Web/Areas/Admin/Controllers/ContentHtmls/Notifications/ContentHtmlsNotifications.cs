using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.ContentHtmls.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.ContentHtml.Notifications
{
    public class ContentHtmlsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<ContentHtmlDto>>(signalRUrlPrefix + "/cms/content-htmls/notifications");
        }
    }
}
