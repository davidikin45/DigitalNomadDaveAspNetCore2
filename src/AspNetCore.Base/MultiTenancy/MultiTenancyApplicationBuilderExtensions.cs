using AspNetCore.Base.MultiTenancy.Data.Tenants;
using AspNetCore.Base.MultiTenancy.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Base.MultiTenancy
{
    public static class MultiTenancyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenants<TTenant>(this IApplicationBuilder builder)
        where TTenant : AppTenant
        {
            return builder.UseMiddleware<TenantMiddleware<TTenant>>();
        }
    }
}
