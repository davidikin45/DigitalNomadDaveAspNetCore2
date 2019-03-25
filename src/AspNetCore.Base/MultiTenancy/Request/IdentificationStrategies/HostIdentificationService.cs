using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Request.IdentificationStrategies
{
    public class HostIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
   where TTenant : AppTenant
    {
        private readonly ILogger<ITenantIdentificationService<TTenant>> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantsStore<TTenant> _store;

        public HostIdentificationService(ITenantsStore<TTenant> store, IHttpContextAccessor contextAccessor, ILogger<ITenantIdentificationService<TTenant>> logger)
        {
            _store = store;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<TTenant> GetTenantAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            //destination
            var host = httpContext.Request.Host.Value.Replace("www.","");
            var hostWithoutPort = host.Split(":")[0];

            //ip restriction security
            var ip = httpContext.Connection.RemoteIpAddress.ToString();

            Func<TTenant, bool> exactMatchHostWithPortCondition = t => t.HostNames.Contains(host);
            Func<TTenant, bool> exactMatchHostWithoutPortCondition = t => t.HostNames.Contains(hostWithoutPort);
            Func<TTenant, bool> endWildcardCondition = t => t.HostNames.Any(h => h.EndsWith("*") && host.StartsWith(h.Replace("*", "")));
            Func<TTenant, bool> startWildcardWithPortCondition = t => t.HostNames.Any(h => h.StartsWith("*") && host.EndsWith(h.Replace("*","")));
            Func<TTenant, bool> startWildcardCondition = t => t.HostNames.Any(h => h.StartsWith("*") && hostWithoutPort.EndsWith(h.Replace("*", "")));

            var tenants = await _store.GetAllTenantsAsync();

            var exactMatchHostWithPort = tenants.Where(exactMatchHostWithPortCondition).ToList();
            var exactMatchHostWithoutPort = tenants.Where(exactMatchHostWithoutPortCondition).ToList();
            var endWildcard = tenants.Where(endWildcardCondition).ToList();
            var startWildcardWithPort = tenants.Where(startWildcardWithPortCondition).ToList();
            var startWildcard = tenants.Where(startWildcardCondition).ToList();

            TTenant tenant = null;
            if(exactMatchHostWithPort.Count > 0)
            {
                if(exactMatchHostWithPort.Count == 1)
                {
                    tenant = exactMatchHostWithPort.First();
                }
            }
            else if(exactMatchHostWithoutPort.Count() > 0)
            {
                if (exactMatchHostWithoutPort.Count == 1)
                {
                    tenant = exactMatchHostWithoutPort.First();
                }
            }
            else if (endWildcard.Count > 0)
            {
                tenant = endWildcard.OrderByDescending(t => t.HostNames.Max(hn => hn.Length)).First();
            }
            else if(startWildcardWithPort.Count > 0)
            {
                tenant = startWildcardWithPort.OrderByDescending(t => t.HostNames.Max(hn => hn.Length)).First();
            }
            else if(startWildcard.Count > 0)
            {
                tenant = startWildcard.OrderByDescending(t => t.HostNames.Max(hn => hn.Length)).First();
            }

            if (tenant != null)
            {
                if(tenant.IpAddressAllowed(ip))
                {
                    this._logger.LogInformation("Identified tenant: {tenant} from host: {host}", tenant.Id, host);
                    httpContext.Items["_tenant"] = tenant;
                    httpContext.Items["_tenantId"] = tenant.Id;
                    return tenant;
                }
            }

            httpContext.Items["_tenant"] = null;
            httpContext.Items["_tenantId"] = null;
            _logger.LogWarning("Unable to identify tenant from host.");
            return null;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
            {
                // No current HttpContext. This happens during app startup
                // and isn't really an error, but is something to be aware of.
                tenantId = null;
                return false;
            }

            // Caching the value both speeds up tenant identification for
            // later and ensures we only see one log message indicating
            // relative success or failure for tenant ID.
            if (httpContext.Items.TryGetValue("_tenantId", out tenantId))
            {
                // We've already identified the tenant at some point
                // so just return the cached value (even if the cached value
                // indicates we couldn't identify the tenant for this context).
                return tenantId != null;
            }

            var tenant = GetTenantAsync(httpContext).GetAwaiter().GetResult();
            if(tenant != null)
            {
                tenantId = tenant.Id;
                return true;
            }

            tenantId = null;
            return false;
        }
    }
}
