using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.ContentTexts.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.ContentText.Notifications
{
    public class ContentTextsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<ContentTextDto>>(signalRUrlPrefix + "/cms/content-texts/notifications");
        }
    }
}
