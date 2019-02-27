using AspNetCore.Base.MultiTenancy.Data.Tenant.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public sealed class FilterTenantDbContext<TDbContext> : IDbContextTenantStrategy<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionStringName => null;

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {

        }

        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
            modelBuilder.AddTenantFilter(tenant.Id, tenantPropertyName);
            modelBuilder.AddTenantShadowPropertyFilter(tenant.Id, tenantPropertyName, true);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

            context.SetTenantIds(tenant.Id, tenantPropertyName);
        }
    }
}
