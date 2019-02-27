using AspNetCore.Base.Authorization;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Api.Authentication
{
    [ResourceCollection(ResourceCollectionsCore.Auth.Name)]
    public abstract class ApiControllerRegistrationBase<TUser, TRegistrationDto> : ApiControllerAuthenticationBase<TUser>
        where TUser : IdentityUser
        where TRegistrationDto : RegisterDtoBase
    {
        private readonly string _welcomeEmailTemplate;

        public ApiControllerRegistrationBase(
            RoleManager<IdentityRole> roleManager,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            TokenSettings tokenSettings,
            LinkGenerator linkGenerator,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings)
            :base(roleManager, userManager, signInManager, tokenSettings, linkGenerator, emailSender, mapper, passwordSettings, emailTemplates, appSettings)
        {
            _welcomeEmailTemplate = emailTemplates.Welcome;
        }

        #region Register
        [ResourceAuthorize(ResourceCollectionsCore.Auth.Operations.Register)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TRegistrationDto registerDto)
        {
            var user = Mapper.Map<TUser>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(_welcomeEmailTemplate))
                {
                    await EmailService.SendWelcomeEmailAsync(_welcomeEmailTemplate, user.Email);
                }
                return await GenerateJWTToken(user);
            }
            AddErrors(result);
            return ValidationErrors();
        }
        #endregion
    }
}
