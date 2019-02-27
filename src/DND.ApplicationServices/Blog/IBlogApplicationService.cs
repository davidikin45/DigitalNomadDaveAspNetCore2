using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.Authors.Services;
using DND.ApplicationServices.Blog.BlogPosts.Services;
using DND.ApplicationServices.Blog.Categories.Services;
using DND.ApplicationServices.Blog.Tags.Services;

namespace DND.ApplicationServices.Blog
{
    public interface IBlogApplicationService : IApplicationService
    {
        IBlogPostApplicationService BlogPostApplicationService { get; }
        ICategoryApplicationService CategoryApplicationService { get; }
        ITagApplicationService TagApplicationService { get; }
        IAuthorApplicationService AuthorApplicationService { get; }
    }
}
