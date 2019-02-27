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
                builder.UseMiddleware<GetRoutes>(routeBuilder);
            });
            return app;
        }
    }

    public class GetRoutes
    {
        private IRouteBuilder _routeBuilder;

        public GetRoutes(RequestDelegate next, IRouteBuilder routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var globals = _routeBuilder?.Routes?.Where(r => r.GetType() != typeof(AttributeRoute)).Select(r => {
                    Route _r = ((Route)(r));
                    return new
                    {
                        _r.Name,
                        Template = _r.RouteTemplate,
                        DefaultAction = _r.Defaults["action"],
                        DefaultController = _r.Defaults["controller"],
                    };
                });

                IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider = context.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>();

                var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(a => new
                {
                    Action = a.RouteValues["action"],
                    Controller = a.RouteValues["controller"],
                    Order = a?.AttributeRouteInfo?.Order,
                    Name = a?.AttributeRouteInfo?.Name,
                    Template = a?.AttributeRouteInfo?.Template,
                    HttpMethods = a?.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods,
                    Authorized = a?.GetCustomAttributes<AuthorizeAttribute>().Any()
                });

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { globals, actions }));
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
