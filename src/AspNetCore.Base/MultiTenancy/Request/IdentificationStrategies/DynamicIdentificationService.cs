using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Request.IdentificationStrategies
{
    public sealed class DynamicTenantIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
        where TTenant : AppTenant
    {
        private readonly Func<HttpContext, TTenant> _currentTenant;
        private readonly Func<IEnumerable<TTenant>> _allTenants;

        private readonly ILogger<ITenantIdentificationService<TTenant>> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public DynamicTenantIdentificationService(IHttpContextAccessor contextAccessor, ILogger<ITenantIdentificationService<TTenant>> logger, Func<HttpContext, TTenant> currentTenant, Func<IEnumerable<TTenant>> allTenants)
        {
            if (currentTenant == null)
            {
                throw new ArgumentNullException(nameof(currentTenant));
            }

            if (allTenants == null)
            {
                throw new ArgumentNullException(nameof(allTenants));
            }
            _contextAccessor = contextAccessor;
            _logger = logger;

            this._currentTenant = currentTenant;
            this._allTenants = allTenants;
        }

        public IEnumerable<TTenant> GetAllTenants()
        {
            return this._allTenants();
        }

        public Task<TTenant> GetTenantAsync(HttpContext httpContext)
        {
            var tenant = this._currentTenant(httpContext);
            httpContext.Items["_tenant"] = tenant;
            httpContext.Items["_tenantId"] = tenant?.Id;

            return Task.FromResult(tenant);
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
