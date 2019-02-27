using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Request.IdentificationStrategies
{
    public class QueryStringIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
    where TTenant : AppTenant
    {
        private readonly ILogger<ITenantIdentificationService<TTenant>> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantsStore<TTenant> _store;

        public QueryStringIdentificationService(ITenantsStore<TTenant> store, IHttpContextAccessor contextAccessor, ILogger<ITenantIdentificationService<TTenant>> logger)
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

            //ip restriction security
            var ip = httpContext.Connection.RemoteIpAddress.ToString();

            var tenantId = httpContext.Request.Query["TenantId"].ToString();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var tenant = await _store.GetTenantByIdAsync(tenantId);
                if (tenant != null)
                {
                    if (tenant.IpAddressAllowed(ip))
                    {
                        httpContext.Items["_tenant"] = tenant;
                        httpContext.Items["_tenantId"] = tenant.Id;
                        _logger.LogInformation("Identified tenant: {tenant} from query string", tenant.Id);
                        return tenant;
                    }
                }
            }

            httpContext.Items["_tenant"] = null;
            httpContext.Items["_tenantId"] = null;
            _logger.LogWarning("Unable to identify tenant from query string.");
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

            var tenant = GetTenantAsync(httpContext).Result;
            if (tenant != null)
            {
                tenantId = tenant.Id;
                return true;
            }

            tenantId = null;
            return false;
        }
    }
}
