using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.Blog.BlogPosts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.Blog
{
    [ResourceCollection(ResourceCollections.Blog.BlogPosts.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/blog/blog-posts")]
    public class BlogPostsController : ApiControllerEntityAuthorizeBase<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IBlogPostApplicationService>
    {
        public BlogPostsController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
