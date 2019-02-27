using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.ApplicationServices.Blog.Categories.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.Blog
{
    [ResourceCollection(ResourceCollections.Blog.Categories.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/blog/categories")]
    public class CategoriesController : ApiControllerEntityAuthorizeBase<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryApplicationService>
    {
        public CategoriesController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
