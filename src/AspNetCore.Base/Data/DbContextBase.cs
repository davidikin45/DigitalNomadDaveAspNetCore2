using AspNetCore.Base.Data.Converters;
using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Data.Migrations;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data
{
    public abstract class DbContextBase : DbContext
    {
        protected DbContextBase()
        {
        }

        public DbContextBase(DbContextOptions options)
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

            builder.RemovePluralizingTableNameConvention();
            builder.AddSoftDeleteFilter();

            //Add Seed Data for things like Enumerations > Lookup Tables. Migrations are generated for this data.

            builder.AddJsonValues();
            builder.AddMultiLangaugeStringValues();
            builder.AddBackingFields();

            BuildQueries(builder);
        }

        public abstract void BuildQueries(ModelBuilder builder);

        #region MSI Access Token
        //https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi
        public string GetMSIAccessTtoken()
        {
            var accessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").GetAwaiter().GetResult();
            return accessToken;
        }
        #endregion

        #region Seed
        public abstract void Seed();
        #endregion

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
