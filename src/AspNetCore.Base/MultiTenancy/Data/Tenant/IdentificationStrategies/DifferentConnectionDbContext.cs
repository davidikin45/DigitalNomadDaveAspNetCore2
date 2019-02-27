using AspNetCore.Base.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public class DifferentConnectionTenantDbContext<TDbContext> : IDbContextTenantStrategy<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionStringName { get; }

        public DifferentConnectionTenantDbContext(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {
            var connectionString = tenant.GetConnectionString(ConnectionStringName);
            optionsBuilder.SetConnectionString<TDbContext>(connectionString);
        }

        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
          
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

        }
    }
}
