using AspNetCore.Base.Controllers.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Identity.Controllers.Authorization
{
    [Area("Identity")]
    [Route("Authorization")]
    public class AuthorizationController : MvcControllerBase
    {
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
