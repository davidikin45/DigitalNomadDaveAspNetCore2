using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Home
{
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : MvcControllerAdminAuthorizeBase
    {

        public HomeController(IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
        }


        public override ActionResult ClearCache()
        {
            return base.ClearCache();
        }

    }
}
