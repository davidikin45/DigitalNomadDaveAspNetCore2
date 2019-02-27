using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Tags.Dtos;
using DND.ApplicationServices.Blog.Tags.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Tags
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.Blog.Tags.CollectionId)]
    [Route("admin/blog/tags")]
    public class TagsController : MvcControllerEntityAuthorizeBase<TagDto, TagDto, TagDto, TagDeleteDto, ITagApplicationService>
    {
        public TagsController(ITagApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
