using AspNetCore.Base.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Routing
{
    //https://gist.github.com/McKabue/b5caf25f9862b2488a9fa7898e22e86e
    public static class GetAllRoutes
    {
        public static IApplicationBuilder AllRoutes(this IRouteBuilder routeBuilder, PathString pathString)
        {
            var app = routeBuilder.ApplicationBuilder;
            app.Map(pathString, builder => {
                builder.UseMiddleware<GetAllRoutesMiddleware>();
            });
            return app;
        }
    }

    public class GetAllRoutesMiddleware
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public GetAllRoutesMiddleware(RequestDelegate next, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var routeInfo = RouteHelper.GetAllRoutes(_actionDescriptorCollectionProvider);

                await context.Response.WriteAsync(JsonConvert.SerializeObject(routeInfo));
                return;
            }
            catch (Exception e)
            {
                await context.Response.WriteAsync($"{e.Message}");
                return;
            }
        }
    }
}
