using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Request.IdentificationStrategies
{
    public class TenantHostQueryStringRequestIpIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
     where TTenant : AppTenant
    {
        private readonly ILogger<ITenantIdentificationService<TTenant>> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITenantsStore<TTenant> _store;

        public TenantHostQueryStringRequestIpIdentificationService(ITenantsStore<TTenant> store, IHttpContextAccessor contextAccessor, ILogger<ITenantIdentificationService<TTenant>> logger)
        {
            _store = store;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<TTenant> GetTenantAsync(HttpContext httpContext)
        {
            var hostIdentificationService = new HostIdentificationService<TTenant>(_store, _contextAccessor, _logger);
            var queryStringIdentificationService = new QueryStringIdentificationService<TTenant>(_store, _contextAccessor, _logger);
            var requestIpIdentificationService = new SourceIPIdentificationService<TTenant>(_store, _contextAccessor, _logger);

            //destination
            var tenant = await hostIdentificationService.GetTenantAsync(httpContext);
            if (tenant != null)
            {
                return tenant;
            }

            tenant = await queryStringIdentificationService.GetTenantAsync(httpContext);
            if (tenant != null)
            {
                return tenant;
            }

            //origin
            tenant = await requestIpIdentificationService.GetTenantAsync(httpContext);

            return tenant;
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
                httpContext.Items["_tenantId"] = tenantId;
                return true;
            }

            tenantId = null;
            httpContext.Items["_tenantId"] = null;
            return false;
        }
    }
}
