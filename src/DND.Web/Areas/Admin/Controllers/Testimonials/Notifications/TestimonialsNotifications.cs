using AspNetCore.Base.SignalR;
using DND.ApplicationServices.CMS.Testimonials.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Areas.Admin.Controllers.Testimonial.Notifications
{
    public class TestimonialsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<TestimonialDto>>(signalRUrlPrefix + "/cms/testimonials/notifications");
        }
    }
}
