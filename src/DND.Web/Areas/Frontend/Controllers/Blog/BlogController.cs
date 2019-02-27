using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Filters;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.MvcServices;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices.Blog;
using DND.Web.Areas.Frontend.Controllers.Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace DND.Web.Areas.Frontend.Controllers.Blog
{
    [Area("Frontend")]
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Blog" })]
    [Route("blog")]
    public class BlogController : MvcControllerBase
    {
        private readonly IBlogApplicationService _blogService;

        public BlogController(IBlogApplicationService blogService, IMapper mapper, IEmailService emailService, AppSettings appSettings)
            : base(mapper, emailService, appSettings)
        {
            _blogService = blogService;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<IActionResult> Posts(int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostApplicationService.GetTotalPostsAsync(true, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts
            };

            ViewBag.PageTitle = "Latest Posts";
            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("archive/{year}/{month}/{title}")]
        public async Task<IActionResult> Post(int year, int month, string title)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var post = await _blogService.BlogPostApplicationService.GetPostAsync(year, month, title, cts.Token);

            if (post == null)
                return NotFound();

            if (post.Published == false && User.Identity.IsAuthenticated == false)
                return Unauthorized();

            return View("Post", post);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("author/{authorSlug}")]
        public async Task<IActionResult> Author(string authorSlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostApplicationService.GetPostsForAuthorAsync(authorSlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostApplicationService.GetTotalPostsForAuthorAsync(authorSlug, cts.Token);
            var authorTask = _blogService.AuthorApplicationService.GetAuthorAsync(authorSlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, authorTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var author = authorTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Author = author
            };

            if (blogPostListViewModel.Author == null)
                return NotFound();

            ViewBag.PageTitle = String.Format(@"Latest posts for Author ""{0}""", blogPostListViewModel.Author.Name);

            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("category/{categorySlug}")]
        public async Task<IActionResult> Category(string categorySlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostApplicationService.GetPostsForCategoryAsync(categorySlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostApplicationService.GetTotalPostsForCategoryAsync(categorySlug, cts.Token);
            var categoryTask = _blogService.CategoryApplicationService.GetCategoryAsync(categorySlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, categoryTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var category = categoryTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Category = category
            };

            if (blogPostListViewModel.Category == null)
                return NotFound();

            if (blogPostListViewModel.Category.Published == false && User.Identity.IsAuthenticated == false)
                return Unauthorized();

            ViewBag.PageTitle = String.Format(@"Latest posts on category ""{0}""", blogPostListViewModel.Category.Name);

            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("tag/{tagSlug}")]
        public async Task<IActionResult> Tag(string tagSlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostApplicationService.GetPostsForTagAsync(tagSlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostApplicationService.GetTotalPostsForTagAsync(tagSlug, cts.Token);
            var TagDtoTask = _blogService.TagApplicationService.GetTagAsync(tagSlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, TagDtoTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var TagDto = TagDtoTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Tag = TagDto
            };

            if (blogPostListViewModel.Tag == null)
                return NotFound();

            ViewBag.PageTitle = String.Format(@"Latest posts tagged on ""{0}""", blogPostListViewModel.Tag.Name);

            return View("PostList", blogPostListViewModel);
        }

        [Route("search")]
        public async Task<ViewResult> Search(string s, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostApplicationService.GetPostsForSearchAsync(s, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostApplicationService.GetTotalPostsForSearchAsync(s, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Search = s
            };

            ViewBag.PageTitle = String.Format(@"Lists of posts found for search text ""{0}""", s);

            return View("PostList", blogPostListViewModel);
        }
    }
}
