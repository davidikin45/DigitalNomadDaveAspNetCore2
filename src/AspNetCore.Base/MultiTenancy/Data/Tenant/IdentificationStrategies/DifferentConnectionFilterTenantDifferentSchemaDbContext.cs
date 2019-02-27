using AspNetCore.Base.Extensions;
using AspNetCore.Base.MultiTenancy.Data.Tenant.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public sealed class DifferentConnectionFilterTenantDifferentSchemaDbContext<TDbContext> : IDbContextTenantStrategy<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionStringName { get; }

        public DifferentConnectionFilterTenantDifferentSchemaDbContext(string connectionStringName)
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
            modelBuilder.AddTenantSchema(tenant.Id);
            modelBuilder.AddTenantFilter(tenant.Id, tenantPropertyName);
            modelBuilder.AddTenantShadowPropertyFilter(tenant.Id, tenantPropertyName, true);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {
            context.SetTenantIds(tenant.Id, tenantPropertyName);
        }
    }
}
