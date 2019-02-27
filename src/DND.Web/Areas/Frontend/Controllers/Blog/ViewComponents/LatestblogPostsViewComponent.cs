using AspNetCore.Base.Helpers;
using DND.ApplicationServices.Blog;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Blog.ViewComponents
{
    public class LatestblogPostsViewComponent : ViewComponent
    {
        private readonly IBlogApplicationService _blogService;

        public LatestblogPostsViewComponent(IBlogApplicationService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(HttpContext.RequestAborted);

            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(0, 6, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask);

            IEnumerable<BlogPostDto> posts = postsTask.Result;

            return View(posts);
        }

    }
}
