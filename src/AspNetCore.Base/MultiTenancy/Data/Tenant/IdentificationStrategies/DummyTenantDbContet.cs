using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public class DummyTenantDbContext<TDbContext> : IDbContextTenantStrategy<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionStringName => null;

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {
 
        }


        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {

        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        { 

        }
    }
}
