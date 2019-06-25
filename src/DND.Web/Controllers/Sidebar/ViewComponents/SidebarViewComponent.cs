using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Settings;
using AspNetCore.Base.ViewComponents;
using AspNetCore.Mvc.Extensions;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.ApplicationServices.Blog.Tags.Dtos;
using DND.Web.Controllers.Sidebar.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Controllers.Sidebar.ViewComponents
{
    public class SidebarViewComponent : ViewComponentBase
    {
        private readonly IBlogApplicationService _blogService;
        private readonly IFileSystemGenericRepositoryFactory FileSystemRepository;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SidebarViewComponent(IBlogApplicationService blogService, IFileSystemGenericRepositoryFactory fileSystemRepository, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
        {
            FileSystemRepository = fileSystemRepository;
            _blogService = blogService;
            _appSettings = appSettings;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = FileSystemRepository.CreateFileRepository(cts.Token, _hostingEnvironment.MapWwwPath(_appSettings.Folders[Folders.Gallery]), true, "*.*", ".jpg", ".jpeg");

            IList<CategoryDto> categories = null;
            IEnumerable<TagDto> tags = null;
            IEnumerable<BlogPostDto> posts = null;
            IEnumerable<FileInfo> photos = null;

            var categoriesTask = _blogService.CategoryApplicationService.GetAsync(cts.Token, c => c.Published);
            var tagsTask = _blogService.TagApplicationService.GetAllAsync(cts.Token);
            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(0, 10, cts.Token);
            var photosTask = repository.GetAllAsync(d => d.OrderByDescending(f => f.LastWriteTime), 0, 6);

            await TaskHelper.WhenAllOrException(cts, tagsTask, categoriesTask);

            List<Task<int>> countTasks = new List<Task<int>>();

            //foreach (TagDto dto in tagsTask.Result)
            //{
            //    tagCountTasks.Add(_blogService.BlogPostService.GetTotalPostsForTagAsync(dto.UrlSlug, cts.Token).ContinueWith(t => dto.Count = t.Result));
            //}
            categories = categoriesTask.Result.ToList();
            foreach (CategoryDto dto in categories)
            {
                countTasks.Add(_blogService.BlogPostApplicationService.GetTotalPostsForCategoryAsync(dto.UrlSlug, cts.Token));
            }

            await TaskHelper.WhenAllOrException(cts, categoriesTask, postsTask, photosTask);
            await TaskHelper.WhenAllOrException(cts, countTasks.ToArray());

            int i = 0;
            foreach (CategoryDto dto in categories)
            {
                dto.Count = countTasks[i].Result;
                i++;
            }

            tags = tagsTask.Result;
            posts = postsTask.Result;
            photos = photosTask.Result;

            var widgetViewModel = new BlogWidgetViewModel
            {
                Categories = categories,
                Tags = tags.ToList(),
                LatestPosts = posts.ToList(),
                LatestPhotos = photos.ToList()
            };

            return View(widgetViewModel);
        }

    }
}
