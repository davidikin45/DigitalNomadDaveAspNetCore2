using AspNetCore.Base.Email;
using AspNetCore.Base.Middleware;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Base.Controllers.Mvc
{
    public abstract class MvcControllerAdminAuthorizeBase : MvcControllerAuthorizeBase
    {

        public MvcControllerAdminAuthorizeBase(IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("file-manager")]
        public ActionResult FileManager()
        {
            return View();
        }

        //https://stackoverflow.com/questions/565239/any-way-to-clear-flush-remove-outputcache/16038654
        [Route("clear-cache")]
        public virtual ActionResult ClearCache()
        {
            ResponseCachingCustomMiddleware.ClearResponseCache();
            return View();
        }
    }
}
