using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCore.Base.Security
{
    public static class JwtSigninManager
    {
        public static void SignIn(HttpResponse response, string token)
        {
            var options = (IOptions<JwtBearerOptions>)response.HttpContext.RequestServices.GetService(typeof(IOptions<JwtBearerOptions>));

            SecurityToken validationToken = null;

            var user = new JwtSecurityTokenHandler().ValidateToken(token, options.Value.TokenValidationParameters, out validationToken);

            response.Cookies.Append(
                "access_token",
                token,
                new CookieOptions()
                {
                    HttpOnly = true,
                    Expires = validationToken.ValidTo,
                    Path = "/"
                }
            );
        }

        //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2
        public static Task SigninAsync(this HttpContext httpContext, string token)
        {
            var options = (IOptions<JwtBearerOptions>)httpContext.RequestServices.GetService(typeof(IOptions<JwtBearerOptions>));

            SecurityToken validationToken = null;

            var user = new JwtSecurityTokenHandler().ValidateToken(token, options.Value.TokenValidationParameters, out validationToken);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = validationToken.ValidTo,
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.

                IssuedUtc = validationToken.ValidFrom,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            return httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                user,
                authProperties);
        }

        public static Task SignOutAsync(this HttpContext httpContext)
        {
            return httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public static void SignOut(HttpResponse response)
        {
           response.Cookies.Delete("access_token");
        }
    }
}
