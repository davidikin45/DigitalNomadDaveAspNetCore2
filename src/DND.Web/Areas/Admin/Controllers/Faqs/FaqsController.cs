using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.Faqs.Dtos;
using DND.ApplicationServices.CMS.Faqs.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Faqs
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.Faqs.CollectionId)]
    [Route("admin/cms/faqs")]
    public class FaqsController : MvcControllerEntityAuthorizeBase<FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqApplicationService>
    {
        public FaqsController(IFaqApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
