using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Base.Middleware
{
    public class ResponseCachingCustomMiddleware : ResponseCachingMiddleware
    {
        public static ResponseCachingCustomMiddleware Instance;

        public ResponseCachingCustomMiddleware(
            RequestDelegate next,
            IOptions<ResponseCachingOptions> options,
            ILoggerFactory loggerFactory,
            IResponseCachingPolicyProvider policyProvider,
            IResponseCachingKeyProvider keyProvider)
            : base(
                next,
                options,
                loggerFactory,
                policyProvider, 
                keyProvider)
        {
            loggerFactory.CreateLogger<ResponseCachingMiddleware>().LogInformation("Response Caching Middleware Initialised");
            Instance = this;
        }

        public static void ClearResponseCache()
        {
            if( Instance != null)
            {
                ResponseCachingOptions options = (ResponseCachingOptions)(typeof(ResponseCachingMiddleware).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Instance));

                long sizeLimit = options.SizeLimit;

                var newCache = new MemoryResponseCache(new MemoryCache(new MemoryCacheOptions
                {
                    SizeLimit = sizeLimit
                }));

                typeof(ResponseCachingMiddleware)
               .GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic)
               .SetValue(Instance, newCache);
            }           
        }
    } 
    
}
