using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.Base.MvcExtensions
{
    public static class LinkExtensions
    {
        public static string BuildUrlFromExpression<TController>(this IHtmlHelper helper, Expression<Action<TController>> action) where TController : ControllerBase
        {
            return LinkBuilder.BuildUrlFromExpression(helper.ViewContext.HttpContext, helper.ViewContext.RouteData.Values, helper.ViewContext.RouteData.Routers.OfType<RouteCollection>().FirstOrDefault(), action);
        }

    }
}
