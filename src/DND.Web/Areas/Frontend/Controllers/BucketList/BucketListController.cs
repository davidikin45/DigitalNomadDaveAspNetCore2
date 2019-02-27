using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.BucketList
{
    [Area("Frontend")]
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "BucketList" })]
    [Route("bucket-list")]
    public class BucketListController : MvcControllerBase
    {
        private readonly IBlogApplicationService _blogService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        private readonly IHostingEnvironment _hostingEnvironment;


        public BucketListController(IBlogApplicationService blogService, IMapper mapper, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IEmailService emailService, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
             : base(mapper, emailService, appSettings)
        {
            _blogService = blogService;
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 100, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = "desc")
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var repository = _fileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, _hostingEnvironment.MapWwwPath(AppSettings.Folders[Folders.BucketList]), true, "*.*", ".jpg", ".jpeg", ".txt", ".mp4", ".avi");
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

    }
}
