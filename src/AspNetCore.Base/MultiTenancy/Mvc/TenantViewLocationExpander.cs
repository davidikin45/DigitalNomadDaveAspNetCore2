using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.MultiTenancy.Mvc
{
    public class TenantViewLocationExpander<TTenant> : IViewLocationExpander
    where TTenant : AppTenant
    {
        private const string ValueKey = "tenantId";

        public TenantViewLocationExpander()
        {
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
           var tenantProvider = (ITenantService<TTenant>)context.ActionContext.HttpContext.RequestServices.GetService(typeof(ITenantService<TTenant>));
            var tenantId = tenantProvider.GetTenant().Id.ToString();
            context.Values[ValueKey] = tenantId;
        }

        //The view locations passed to ExpandViewLocations are:
        // /Views/{1}/{0}.cshtml
        // /Shared/{0}.cshtml
        // /Pages/{0}.cshtml
        //Where {0} is the view and {1} the controller name.
        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            foreach (var location in viewLocations)
            {
                yield return location.Replace("{0}", context.Values[ValueKey] + "/{0}");
                yield return location;
            }
        }
    }
}
