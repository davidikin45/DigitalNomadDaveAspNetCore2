using AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public static class DbContextIdentificationStrategyExtensions
    {
        public static IServiceCollection DifferentConnectionForTenant<TDbContext>(this TenantDbContextIdentification<TDbContext> identification, string connectionStringName)
            where TDbContext : DbContext
        {
            return identification._services.AddSingleton<IDbContextTenantStrategy<TDbContext>>(x => new DifferentConnectionTenantDbContext<TDbContext>(connectionStringName));
        }

        public static IServiceCollection AllowDifferentSchemaForTenant<TDbContext>(this TenantDbContextIdentification<TDbContext> identification)
             where TDbContext : DbContext
        {
            return identification._services.AddSingleton<IDbContextTenantStrategy<TDbContext>, DifferentSchemaTenantDbContex<TDbContext>>();
        }

        public static IServiceCollection AllowFilterByTenant<TDbContext>(this TenantDbContextIdentification<TDbContext> identification)
              where TDbContext : DbContext
        {
            return identification._services.AddSingleton<IDbContextTenantStrategy<TDbContext>, FilterTenantDbContext<TDbContext>>();
        }

        public static IServiceCollection AllowDifferentConnectionFilterByTenantAndDifferentSchemaForTenant<TDbContext>(this TenantDbContextIdentification<TDbContext> identification, string connectionStringName)
              where TDbContext : DbContext
        {
            return identification._services.AddSingleton<IDbContextTenantStrategy<TDbContext>>(x => new DifferentConnectionFilterTenantDifferentSchemaDbContext<TDbContext>(connectionStringName));
        }

        public static IServiceCollection Dummy<TDbContext>(this TenantDbContextIdentification<TDbContext> identification)
              where TDbContext : DbContext
        {
            return identification._services.AddSingleton<IDbContextTenantStrategy<TDbContext>, DummyTenantDbContext<TDbContext>>();
        }
    }
}
