using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.Security
{
    public static class JWTInHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtCookieAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieMiddleware>();
        }
    }

    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var accessTokenCookie = context.Request.Cookies["access_token"];
            if (accessTokenCookie != null)
            {
                context.Request.Headers.Remove("Authorization");
                context.Request.Headers.Append("Authorization", $"Bearer {accessTokenCookie}");
            }

            await _next.Invoke(context);
        }
    }
}
