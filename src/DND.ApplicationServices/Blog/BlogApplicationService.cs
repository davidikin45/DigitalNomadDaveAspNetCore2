using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.Authors.Services;
using DND.ApplicationServices.Blog.BlogPosts.Services;
using DND.ApplicationServices.Blog.Categories.Services;
using DND.ApplicationServices.Blog.Tags.Services;
using Microsoft.AspNetCore.Authorization;

namespace DND.ApplicationServices.Blog
{
    public class BlogApplicationService : ApplicationServiceBase, IBlogApplicationService
    {
        public IBlogPostApplicationService BlogPostApplicationService { get; private set; }
        public ICategoryApplicationService CategoryApplicationService { get; private set; }
        public ITagApplicationService TagApplicationService { get; private set; }
        public IAuthorApplicationService AuthorApplicationService { get; private set; }

        public BlogApplicationService(IMapper mapper,
            IBlogPostApplicationService blogPostApplicationService,
            ICategoryApplicationService categoryApplicationService,
            ITagApplicationService tagApplicationService,
            IAuthorApplicationService authorApplicationService,
            IAuthorizationService authorizationService,
            IUserService userService,
            IValidationService validationService)
            : base(mapper, authorizationService, userService, validationService)
        {
            BlogPostApplicationService = blogPostApplicationService;
            CategoryApplicationService = categoryApplicationService;
            TagApplicationService = tagApplicationService;
            AuthorApplicationService = authorApplicationService;
        }

    }
}
