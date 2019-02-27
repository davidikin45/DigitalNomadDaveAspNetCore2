using AspNetCore.Base.Controllers.Api.Authentication;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/auth")]
    public class AuthController : ApiControllerAuthenticationBase<User>
    {
        public AuthController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings,
            LinkGenerator linkGenerator,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings)
            : base(roleManager, userManager, signInManager, tokenSettings, linkGenerator, emailSender, mapper, passwordSettings, emailTemplates, appSettings)
        {

        }
    }
}
