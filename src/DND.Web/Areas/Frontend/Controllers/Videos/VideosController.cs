using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Filters;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.Settings;
using AspNetCore.Mvc.Extensions;
using AutoMapper;
using DND.ApplicationServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Videos
{
    [Area("Frontend")]
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Videos" })]
    [Route("videos")]
    public class VideosController : MvcControllerBase
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        private readonly IHostingEnvironment _hostingEnvironment;

        public VideosController(IMapper mapper, IEmailService emailService, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
             : base(mapper, emailService, appSettings)
        {
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        [NoAjaxRequest]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = "desc")
        {
            try
            {

                string physicalPath = _hostingEnvironment.MapWwwPath(AppSettings.Folders[Folders.Videos]);

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetVideosViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                ViewBag.Album = new DirectoryInfo(_hostingEnvironment.MapWwwPath(AppSettings.Folders[Folders.Videos]));

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

        [AjaxRequest]
        [ActionName("Index")]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public virtual async Task<ActionResult> IndexList(int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = "desc")
        {
            try
            {
                string physicalPath = _hostingEnvironment.MapWwwPath(AppSettings.Folders[Folders.Videos]);

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetVideosViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                return PartialView("_VideoAjax", response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        private async Task<WebApiPagedResponseDto<FileInfo>> GetVideosViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = "desc")
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".mp4", ".avi", ".txt");
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
