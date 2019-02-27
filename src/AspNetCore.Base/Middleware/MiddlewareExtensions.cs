using AspNetCore.Base.Hangfire;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Middleware.ImageProcessing;
using AspNetCore.Base.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace AspNetCore.Base.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(
           this IApplicationBuilder builder)
        {
           return builder.UseMiddleware<BasicAuthMiddleware>();
        }

        public static IApplicationBuilder UseRequestTasks(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTasksMiddleware>();
        }

        public static IApplicationBuilder UseContentHandler(
           this IApplicationBuilder builder, IHostingEnvironment env, AppSettings appSettings, List<string> publicUploadFolders, int cacheDays)
        {
            return builder.UseMiddleware<ContentHandlerMiddleware>(env, publicUploadFolders, appSettings, cacheDays);
        }

        public static IApplicationBuilder UseResponseCachingCustom(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseCachingCustomMiddleware>();
        }

        public static IApplicationBuilder UseVersionedStaticFiles(
         this IApplicationBuilder app, int days)
        {
            return app.UseWhen(context => context.Request.Query.ContainsKey("v"),
                   appBranch =>
                   {
                      //cache js, css
                      appBranch.UseStaticFiles(new StaticFileOptions
                       {
                           OnPrepareResponse = ctx =>
                           {
                               if (days > 0)
                               {
                                   TimeSpan timeSpan = new TimeSpan(days * 24, 0, 0);
                                   ctx.Context.Response.GetTypedHeaders().Expires = DateTime.Now.Add(timeSpan).Date.ToUniversalTime();
                                   ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                   {
                                       Public = true,
                                       MaxAge = timeSpan
                                   };
                               }
                               else
                               {
                                   ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                   {
                                       NoCache = true
                                   };
                               }
                           }
                       });
                   }
              );
        }

        public static IApplicationBuilder UseNonVersionedStaticFiles(
       this IApplicationBuilder app, int days)
        {
            return app.UseWhen(context => !context.Request.Query.ContainsKey("v"),
                   appBranch =>
                   {
                      //cache js, css
                      appBranch.UseStaticFiles(new StaticFileOptions
                       {
                           OnPrepareResponse = ctx =>
                           {
                               if (days > 0)
                               {
                                   TimeSpan timeSpan = new TimeSpan(days * 24, 0, 0);
                                   ctx.Context.Response.GetTypedHeaders().Expires = DateTime.Now.Add(timeSpan).Date.ToUniversalTime();
                                   ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                   {
                                       Public = true,
                                       MaxAge = timeSpan
                                   };
                               }
                               else
                               {
                                   ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                   {
                                       NoCache = true
                                   };
                               }
                           }
                       });
                   }
              );
        }

        public static IApplicationBuilder UseStackifyPrefix(this IApplicationBuilder app)
        {
            return app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
        }
    }
}
