using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.ApiClient
{
    public class AuthorizationJwtProxyHttpHandler : DelegatingHandler
    {
        private readonly string accessToken;
        public AuthorizationJwtProxyHttpHandler(IHttpContextAccessor httpContextAccessor)
        {
            accessToken =  httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (accessToken != null)
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
