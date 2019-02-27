using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.MailingLists.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.MailingList.Notifications
{
    public class MailingListNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<MailingListDto>>(signalRUrlPrefix + "/cms/mailing-list/notifications");
        }
    }
}
