using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AspNetCore.Base.ApiClient
{
    public static class PolicyHolder
    {
        public static AsyncPolicyWrap<HttpResponseMessage> GetRequestPolicy(IMemoryCache memoryCache = null, int cacheSeconds = 0, int additionalRetries = 0, int requestTimeoutSeconds = 100)
        {
            AsyncCachePolicy cache = null;
            if (memoryCache != null)
            {
                var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
                cache = Policy.CacheAsync(memoryCacheProvider, TimeSpan.FromSeconds(cacheSeconds));
            }

            HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout, // 408
               HttpStatusCode.TooManyRequests, // 429
               //HttpStatusCode.InternalServerError, // 500
               HttpStatusCode.BadGateway, // 502
               HttpStatusCode.ServiceUnavailable, // 503
               HttpStatusCode.GatewayTimeout // 504
            };

            var waitAndRetryPolicy = Policy
             .Handle<HttpRequestException>() //HttpClient Timeout or CancellationToken
             .Or<TimeoutRejectedException>()
             .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
             .WaitAndRetryAsync(additionalRetries,
              retryAttempt => TimeSpan.FromSeconds(1));

            //https://github.com/App-vNext/Polly/wiki/Timeout
            var requestTimeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(requestTimeoutSeconds));

            //https://github.com/App-vNext/Polly/wiki/PolicyWrap
            AsyncPolicyWrap<HttpResponseMessage> policyWrap = null;
            if (cache != null)
            {
                policyWrap = cache.WrapAsync(waitAndRetryPolicy).WrapAsync(requestTimeout);
            }
            else
            {
                policyWrap = waitAndRetryPolicy.WrapAsync(requestTimeout);
            }

            return policyWrap;
        }
    }
}
