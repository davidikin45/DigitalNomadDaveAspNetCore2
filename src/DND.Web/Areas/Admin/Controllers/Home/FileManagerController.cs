using AspNetCore.Base.Controllers.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Home
{
    [Area("Admin")]
    public class FileManagerController : MvcControllerAdminFileManagerAuthorizeBase
    {
        public FileManagerController(IHostingEnvironment hostingEnvironment)
            :base(hostingEnvironment)
        {

        }

    }
}
