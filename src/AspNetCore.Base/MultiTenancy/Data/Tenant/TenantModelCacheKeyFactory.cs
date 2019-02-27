using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            if (context is IDbContextTenantBase dynamicContext)
            {
                var tenantService = dynamicContext.TenantService;
                var tenanantId = tenantService.GetTenantId();
                if( tenanantId != null )
                {
                    return new { tenanantId };
                }
            }

            return new ModelCacheKey(context);
        }
    }
}
