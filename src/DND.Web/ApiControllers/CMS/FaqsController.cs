using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.Faqs.Dtos;
using DND.ApplicationServices.CMS.Faqs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.CMS
{
    [ResourceCollection(ResourceCollections.CMS.Faqs.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/cms/faqs")]
    public class FaqsController : ApiControllerEntityAuthorizeBase<FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqApplicationService>
    {
        public FaqsController(IFaqApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
