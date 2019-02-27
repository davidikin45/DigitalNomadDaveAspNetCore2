using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.Common.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Home
{
    [Area("Admin")]
    [Route("admin/folder-metadata")]
    public class FolderMetadataController : MvcControllerFolderMetadataAuthorizeBase
    {
        public FolderMetadataController(IFileSystemGenericRepositoryFactory fileSytemGenericRepositoryFactory, IMapper mapper, IEmailService emailService, AppSettings appSettings, IHostingEnvironment hostingEnvironment)
             : base(hostingEnvironment.MapWwwPath(appSettings.Folders[Folders.Files]), true, true, fileSytemGenericRepositoryFactory, mapper, emailService)
        {

        }
    }
}
