using AspNetCore.Base.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public abstract class DbContextIdentityTentantBase<TUser> : DbContextIdentityBase<TUser>, IDbContextTenantBase where TUser : IdentityUser
    {
        public DbContextIdentityTentantBase(DbContextOptions options, ITenantService tenantService)
            :base(options)
        {
            TenantService = tenantService;
        }

        public ITenantService TenantService { get; }

        public DbContextIdentityTentantBase(ITenantService tenantService)
        {
            TenantService = tenantService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            TenantService?.GetTenantStrategy(this)?.OnConfiguring(optionsBuilder, TenantService.GetTenant(), TenantService.TenantPropertyName);
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            TenantService?.GetTenantStrategy(this)?.OnModelCreating(modelBuilder, this, TenantService.GetTenant(), TenantService.TenantPropertyName);
        }

        #region Save Changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            TenantService?.GetTenantStrategy(this)?.OnSaveChanges(this, TenantService.GetTenant(), TenantService.TenantPropertyName);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            TenantService?.GetTenantStrategy(this)?.OnSaveChanges(this, TenantService.GetTenant(), TenantService.TenantPropertyName);

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            TenantService?.GetTenantStrategy(this)?.OnSaveChanges(this, TenantService.GetTenant(), TenantService.TenantPropertyName);

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken
            = default(CancellationToken))
        {
            TenantService?.GetTenantStrategy(this)?.OnSaveChanges(this, TenantService.GetTenant(), TenantService.TenantPropertyName);

            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}
