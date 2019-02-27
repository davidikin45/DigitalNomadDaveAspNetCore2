using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Localization
{
    public class RedirectUnsupportedCulturesMiddleware
    {
        private readonly bool _redirectCultureless;
        private readonly RequestDelegate _next;
        private readonly string _routeDataStringKey;

        public RedirectUnsupportedCulturesMiddleware(
            RequestDelegate next,
            RequestLocalizationOptions options, 
            bool redirectCultureless)
        {
            _next = next;
            var provider = options.RequestCultureProviders
                .Select(x => x as RouteDataRequestCultureProvider)
                .Where(x => x != null)
                .FirstOrDefault();
            _routeDataStringKey = provider.RouteDataStringKey;
            _redirectCultureless = redirectCultureless;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestedCulture = context.GetRouteValue(_routeDataStringKey)?.ToString();
            var cultureFeature = context.Features.Get<IRequestCultureFeature>();

            var actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            if ((_redirectCultureless && string.IsNullOrEmpty(requestedCulture)) || (!string.IsNullOrEmpty(requestedCulture) && !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase)))
            {
                var newCulturedPath = GetNewPath(context, actualCulture);
                context.Response.Redirect(newCulturedPath);
                return;
            }

            await _next.Invoke(context);
        }

        private string GetNewPath(HttpContext context, string newCulture)
        {
            var routeData = context.GetRouteData();
            var router = routeData.Routers[0];
            var virtualPathContext = new VirtualPathContext(
                context,
                routeData.Values,
                new RouteValueDictionary { { _routeDataStringKey, newCulture } });

            return router.GetVirtualPath(virtualPathContext).VirtualPath;
        }
    }
}
