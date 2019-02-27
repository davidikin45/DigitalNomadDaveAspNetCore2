using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.ApplicationServices.Blog.Locations.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Locations
{
    [Area("Frontend")]
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Locations" })]
    [Route("locations")]
    public class LocationsController : MvcControllerBase
    {
        private readonly ILocationApplicationService _locationService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LocationsController(ILocationApplicationService locationService, IMapper mapper, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IEmailService emailService, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
             : base(mapper, emailService, appSettings)
        {
            _locationService = locationService;
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string orderColumn = nameof(LocationDto.Name), string orderType = "asc", string search = "")
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = _locationService.SearchAsync(cts.Token, null, search, l => !string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug), AutoMapperHelper.GetOrderBy<LocationDto>(orderColumn, orderType), page - 1, pageSize);
                var totalTask = _locationService.GetSearchCountAsync(cts.Token, null, search, l => !string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug));

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<LocationDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                return View(response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        // GET: Default/Details/5
        [Route("{urlSlug}")]
        public virtual async Task<ActionResult> Location(string urlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            try
            {
                var data = await _locationService.GetLocationAsync(urlSlug, cts.Token);

                if (data == null)
                    return NotFound();

                //TODO: Locations need image paging
                string physicalPath = _hostingEnvironment.MapWwwPath(AppSettings.Folders[Folders.Gallery]) + data.Album;

                return View("Location", data);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw ex;
                }
                else
                {
                    return HandleReadException();
                }
            }

        }

        private async Task<WebApiPagedResponseDto<FileInfo>> GetLocationViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".jpg", ".jpeg", ".mp4", ".avi", ".txt");
            var dataTask = repository.GetAllAsync(AutoMapperHelper.GetOrderByFunc<FileInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
            var totalTask = repository.GetCountAsync(null);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var response = new WebApiPagedResponseDto<FileInfo>
            {
                Page = page,
                PageSize = pageSize,
                Records = total,
                Rows = data.ToList(),
                OrderColumn = orderColumn,
                OrderType = orderType
            };

            return response;
        }

    }
}
