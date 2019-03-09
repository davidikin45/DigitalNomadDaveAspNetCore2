using AspNetCore.Base.Authentication;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Email;
using AspNetCore.Base.Security;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Users;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.Api.Authentication
{
    [ResourceCollection(ResourceCollectionsCore.Auth.Name)]
    public abstract class ApiControllerAuthenticationBase<TUser> : ApiControllerBase
        where TUser : IdentityUser
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        protected readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;

        private readonly string _privateSymmetricKey;
        private readonly string _privateSigningKeyPath;
        private readonly string _privateSigningCertificatePath;
        private readonly string _privateSigningCertificatePassword;
        private readonly string _localIssuer;

        private readonly string _passwordResetCallbackUrl;

        private readonly string _resetPasswordEmailTemplate;

        private readonly int _tokenExpiryMinutes;

        public ApiControllerAuthenticationBase(
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
            :base(mapper, emailSender, linkGenerator, appSettings)
        {
            _resetPasswordEmailTemplate = emailTemplates.ResetPassword;

            _passwordResetCallbackUrl = passwordSettings.PasswordResetCallbackUrl;

            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;

            _privateSymmetricKey = tokenSettings.Key;
            _privateSigningKeyPath = tokenSettings.PrivateKeyPath;
            _privateSigningCertificatePath = tokenSettings.PrivateCertificatePath;
            _privateSigningCertificatePassword = tokenSettings.PrivateCertificatePasword;

            _localIssuer = tokenSettings.LocalIssuer;
            _tokenExpiryMinutes = tokenSettings.ExpiryMinutes;
        }

        #region Authenticate
        [ResourceAuthorize(ResourceCollectionsCore.Auth.Operations.Authenticate)]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto authenticateDto)
        {
            var user = await _userManager.FindByEmailAsync(authenticateDto.Email);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, authenticateDto.Password, true);

                if (result.Succeeded)
                {
                    return await GenerateJWTToken(user);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return ValidationErrors(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return ValidationErrors(ModelState);
            }
        }
        #endregion

        #region Generate JWT Token
        protected async Task<IActionResult> GenerateJWTToken(TUser user)
        {
            var rolesAndScopes = await AuthenticationHelper.GetRolesAndScopesAsync(user, _userManager, _roleManager);
            var roles = rolesAndScopes.Roles;
            var scopes = rolesAndScopes.Scopes;

            if (!string.IsNullOrWhiteSpace(_privateSigningKeyPath))
            {
                var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);
                var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, key, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
            else if (!string.IsNullOrWhiteSpace(_privateSigningCertificatePassword))
            {
                var key = SigningKey.LoadPrivateSigningCertificate(_privateSigningCertificatePassword, _privateSigningCertificatePassword);
                var results = JwtTokenHelper.CreateJwtTokenSigningWithCertificateSecurityKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, key, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
            else
            {
                var results = JwtTokenHelper.CreateJwtTokenSigningWithKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, _privateSymmetricKey, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
        }
        #endregion

        #region Forgot Password
        [ResourceAuthorize(ResourceCollectionsCore.Auth.Operations.ForgotPassword)]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return Ok();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (!string.IsNullOrEmpty(_resetPasswordEmailTemplate))
            {
                await EmailService.SendResetPasswordEmailAsync(Url, _resetPasswordEmailTemplate, forgotPasswordDto.Email, _passwordResetCallbackUrl, user.Id, code, Request.Scheme);
            }

            return Ok();
        }
        #endregion

        #region Reset Password
        [ResourceAuthorize(ResourceCollectionsCore.Auth.Operations.ResetPassword)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code, resetPasswordDto.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                AddErrors(result);
                return ValidationErrors();
            }
            else
            {
                return Ok();
            }
        }
        #endregion

        #region Helpers
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}
