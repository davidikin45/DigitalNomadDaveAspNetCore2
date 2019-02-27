using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Authors.Dtos;
using DND.ApplicationServices.Blog.Authors.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.Blog
{
    [ResourceCollection(ResourceCollections.Blog.Authors.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/blog/authors")]
    public class AuthorsController : ApiControllerEntityAuthorizeBase<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AuthorsController(IAuthorApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
