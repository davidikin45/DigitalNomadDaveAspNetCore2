using AspNetCore.Base.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq.Expressions;

namespace AspNetCore.Base.MvcExtensions
{
    public static class LinkBuilder
    {
        public static string BuildUrlFromExpression<TController>(HttpContext httpContext, RouteValueDictionary ambientValues, RouteCollection routeCollection, Expression<Action<TController>> action) where TController : ControllerBase
        {
            var result = ExpressionHelper.GetRouteValuesFromExpression(action);
            VirtualPathContext context = new VirtualPathContext(httpContext, ambientValues, result.RouteValues);
            VirtualPathData vpd = routeCollection.GetVirtualPath(context);
            return (vpd == null) ? null : vpd.VirtualPath;
        }

    }
}
