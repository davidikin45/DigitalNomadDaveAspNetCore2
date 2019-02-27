using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public interface IDbContextTenantStrategy<TDbContext> : IDbContextTenantStrategy
        where TDbContext : DbContext
    {

    }

    public interface IDbContextTenantStrategy
    {
        string ConnectionStringName { get; }
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant appTenant, string tenantPropertyName);
        void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant appTenant, string tenantPropertyName);
        void OnSaveChanges(DbContext tenantDbContext, AppTenant appTenant, string tenantPropertyName);
    }
}
