using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.ContentHtmls.Dtos;
using DND.ApplicationServices.CMS.ContentHtmls.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.ContentHtmls
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.ContentHtmls.CollectionId)]
    [Route("admin/cms/content-htmls")]
    public class ContentHtmlsController : MvcControllerEntityAuthorizeBase<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IContentHtmlApplicationService>
    {
        public ContentHtmlsController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }

    }
}
