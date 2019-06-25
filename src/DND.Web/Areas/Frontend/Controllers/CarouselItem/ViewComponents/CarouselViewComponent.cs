using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.Settings;
using AspNetCore.Base.ViewComponents;
using AspNetCore.Mvc.Extensions;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;
using DND.ApplicationServices.CMS.CarouselItems.Services;
using DND.Web.Areas.Frontend.Controllers.CarouselItem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.CarouselItem.ViewComponents
{
    [ViewComponent]
    public class CarouselViewComponent : ViewComponentBase
    {
        private readonly IBlogApplicationService _blogService;
        private readonly ICarouselItemApplicationService _carouselItemService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemRepository;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CarouselViewComponent(IBlogApplicationService blogService, ICarouselItemApplicationService carouselItemService, IFileSystemGenericRepositoryFactory fileSystemRepository, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
        {
            _fileSystemRepository = fileSystemRepository;
            _blogService = blogService;
            _carouselItemService = carouselItemService;
            _appSettings = appSettings;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(CarouselItemDto.CreatedOn);
            string orderType = "desc";

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<BlogPostDto> posts = null;
            IList<CarouselItemDto> carouselItemsFinal = new List<CarouselItemDto>();
            IEnumerable<CarouselItemDto> carouselItems = null;

            IList<DirectoryInfo> albums = new List<DirectoryInfo>();
            IList<CarouselItemDto> albumCarouselItems = new List<CarouselItemDto>();


            var postsTask = _blogService.BlogPostApplicationService.GetPostsForCarouselAsync(0, 3, cts.Token);
            var carouselItemsTask = _carouselItemService.GetAsync(cts.Token, c => c.Published, AutoMapperHelper.GetOrderBy<CarouselItemDto>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, postsTask, carouselItemsTask);

            posts = postsTask.Result;
            carouselItems = carouselItemsTask.Result;

            var repository = _fileSystemRepository.CreateFolderRepositoryReadOnly(cts.Token, _hostingEnvironment.MapWwwPath(_appSettings.Folders[Folders.Gallery]));
            foreach (CarouselItemDto item in carouselItems)
            {
                if (!string.IsNullOrEmpty(item.Album))
                {
                    var album = repository.GetByPath(item.Album);
                    if (album != null)
                    {
                        albums.Add(album);
                        albumCarouselItems.Add(item);
                    }
                }
                else
                {
                    carouselItemsFinal.Add(item);
                }
            }

            var carouselViewModel = new CarouselViewModel
            {
                Posts = posts.ToList(),

                Albums = albums.ToList(),
                AlbumCarouselItems = albumCarouselItems.ToList(),

                CarouselItems = carouselItemsFinal.ToList(),
                ItemCount = posts.Count() + albums.Count() + carouselItemsFinal.Count()
            };

            return View(carouselViewModel);
        }

    }
}
