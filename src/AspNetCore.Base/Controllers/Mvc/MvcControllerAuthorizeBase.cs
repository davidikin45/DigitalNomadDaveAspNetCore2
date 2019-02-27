using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.Base.Controllers.Mvc
{
    [Authorize(Roles = "admin")]
    public abstract class MvcControllerAuthorizeBase : MvcControllerBase
    {
        public MvcControllerAuthorizeBase()
        {

        }

        public MvcControllerAuthorizeBase(IMapper mapper = null, IEmailService emailService = null, AppSettings appSettings = null)
            :base(mapper, emailService, appSettings)
        {
         
        }
    }
}
