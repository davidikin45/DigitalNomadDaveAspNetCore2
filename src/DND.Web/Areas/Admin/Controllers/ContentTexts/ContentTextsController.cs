using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.ContentTexts.Dtos;
using DND.ApplicationServices.CMS.ContentTexts.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.ContentTexts
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.ContentTexts.CollectionId)]
    [Route("admin/cms/content-texts")]
    public class ContentTextsController : MvcControllerEntityAuthorizeBase<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextApplicationService>
    {
        public ContentTextsController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }


    }
}
