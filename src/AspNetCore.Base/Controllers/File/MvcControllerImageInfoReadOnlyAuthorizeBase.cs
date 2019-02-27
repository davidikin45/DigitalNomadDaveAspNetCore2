using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Data.RepositoryFileSystem.File;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.File
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class MvcControllerImageInfoReadOnlyAuthorizeBase : MvcControllerBase
    {   
        public IFileSystemGenericRepositoryFactory FileSystemGenericRepositoryFactory { get; private set; }
        public Boolean Admin { get; set; }
        public Boolean IncludeSubDirectories { get; set; }
        public String PhysicalPath { get; set; }

        public MvcControllerImageInfoReadOnlyAuthorizeBase(string physicalPath, Boolean includeSubDirectories, Boolean admin, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper = null, IEmailService emailService = null, AppSettings appSettings = null)
        : base(mapper, emailService, appSettings)
        {
            PhysicalPath = physicalPath;
            IncludeSubDirectories = includeSubDirectories;
            Admin = admin;
            FileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        // GET: Default
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = "desc",string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
                  
            try
            {
                var repository = FileSystemGenericRepositoryFactory.CreateImageInfoRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                var dataTask = repository.MetadataSearchAsync(search, null, AutoMapperHelper.GetOrderByFunc<FileInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
                var totalTask = repository.GetSearchCountAsync(search, null);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var rows = data.ToList().Select(Mapper.Map<ImageInfo, ImageInfoDto>).ToList();

                var response = new WebApiPagedResponseDto<ImageInfoDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = rows,
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search

                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                ViewBag.DisableCreate = true;
                ViewBag.DisableSorting = true;
                ViewBag.DisableDelete = false;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        // GET: Default/Details/5
        [Route("details/{*id}")]
        public virtual async Task<ActionResult> Details(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            ImageInfo data = null;
            try
            {
                var repository = FileSystemGenericRepositoryFactory.CreateImageInfoRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);

                data = await repository.MetadataGetByPathAsync(id.Replace("/","\\"));

                if (data == null)
                    return HandleReadException();
            }
            catch
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;

            var dto = Mapper.Map<ImageInfoDto>(data);

            return View("Details", dto);
        }  

    }
}

