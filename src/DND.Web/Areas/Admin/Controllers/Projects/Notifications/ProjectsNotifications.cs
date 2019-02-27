using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.Projects.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Project.Notifications
{
    public class ProjectsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<ProjectDto>>(signalRUrlPrefix + "/cms/projects/notifications");
        }
    }
}
