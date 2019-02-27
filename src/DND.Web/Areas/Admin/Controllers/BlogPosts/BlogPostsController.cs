using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.Blog.BlogPosts.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.BlogPosts
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.Blog.BlogPosts.CollectionId)]
    [Route("admin/blog/blog-posts")]
    public class BlogPostsController : MvcControllerEntityAuthorizeBase<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IBlogPostApplicationService>
    {
        public BlogPostsController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
