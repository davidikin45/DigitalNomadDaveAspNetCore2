using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.Testimonials.Dtos;
using DND.ApplicationServices.CMS.Testimonials.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Testimonials
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.Testimonials.CollectionId)]
    [Route("admin/cms/testimonials")]
    public class TestimonialsController : MvcControllerEntityAuthorizeBase<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialApplicationService>
    {
        public TestimonialsController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
