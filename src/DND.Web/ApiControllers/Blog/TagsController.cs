using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Tags.Dtos;
using DND.ApplicationServices.Blog.Tags.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.Blog
{
    [ResourceCollection(ResourceCollections.Blog.Tags.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/blog/tags")]
    public class TagsController : ApiControllerEntityAuthorizeBase<TagDto, TagDto, TagDto, TagDeleteDto, ITagApplicationService>
    {
        public TagsController(ITagApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }

    }
}
