using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.ApiClient
{
    public class AuthorizationBearerProxyHttpHandler : DelegatingHandler
    {
        private readonly string bearerToken;
        public AuthorizationBearerProxyHttpHandler(IHttpContextAccessor httpContextAccessor)
        {
            bearerToken = httpContextAccessor.HttpContext?.Request
                      .Headers["Authorization"]
                      .FirstOrDefault(h => h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (bearerToken != null)
                request.Headers.Add("Authorization", bearerToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
