using AspNetCore.Base.MultiTenancy.Data.Tenant.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public class DifferentSchemaTenantDbContex<TDbContext> : IDbContextTenantStrategy<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionStringName => null;

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {
         
        }

        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
            modelBuilder.AddTenantSchema(tenant.Id, true);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

        }
    }
}
