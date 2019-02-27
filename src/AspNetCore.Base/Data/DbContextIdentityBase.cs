using AspNetCore.Base.Data.Converters;
using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Data.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.UnitOfWork
{
    public abstract class DbContextIdentityBase<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
    {
        protected DbContextIdentityBase()
        {

        }

        public DbContextIdentityBase(DbContextOptions options)
            : base(options)
        {

        }

        public bool LazyLoadingEnabled
        {
            get { return ChangeTracker.LazyLoadingEnabled; }
            set { ChangeTracker.LazyLoadingEnabled = value; }
        }

        public bool AutoDetectChangesEnabled
        {
            get { return ChangeTracker.AutoDetectChangesEnabled; }
            set { ChangeTracker.AutoDetectChangesEnabled = value; }
        }

        public QueryTrackingBehavior DefaultQueryTrackingBehavior
        {
            get { return ChangeTracker.QueryTrackingBehavior; }
            set { ChangeTracker.QueryTrackingBehavior = value; }
        }

        public static readonly ILoggerFactory CommandLoggerFactory
        = new ServiceCollection().AddLogging(builder =>
        {
            builder.AddDebug().AddConsole().AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
        }).BuildServiceProvider()
        .GetService<ILoggerFactory>();

        public static readonly ILoggerFactory ChangeTrackerLoggerFactory
         = new ServiceCollection().AddLogging(builder =>
         {
             builder.AddDebug().AddConsole().AddFilter(DbLoggerCategory.ChangeTracking.Name, LogLevel.Debug);
         }).BuildServiceProvider()
         .GetService<ILoggerFactory>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            optionsBuilder.UseLoggerFactory(CommandLoggerFactory).EnableSensitiveDataLogging();
            optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, RelationalMigrationsAnnotationsProvider>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.HasDefaultSchema("dbo"); //SQLite doesnt have schemas

            builder.RemovePluralizingTableNameConvention();
            builder.AddSoftDeleteFilter();

            builder.AddJsonValues();
            builder.AddMultiLangaugeStringValues();
            builder.AddBackingFields();

            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            builder.Entity<TUser>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");

            BuildQueries(builder);
        }

        public abstract void BuildQueries(ModelBuilder builder);

        #region Migrate
        public void Migrate()
        {
            Database.Migrate();
        }
        #endregion

        #region Save Changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.SetTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            this.SetTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.SetTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken
            = default(CancellationToken))
        {
            this.SetTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion


    }
}
