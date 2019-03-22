using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using Database.Initialization;
using DND.Data;
using DND.Data.Initializers;
using Hangfire.Initialization;
using Microsoft.EntityFrameworkCore;
using MiniProfiler.Initialization;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTests.Data
{
    public class SqlServerDbTests
    {
        [Fact]
        public async Task Initialization()
        {
            //db location = C:\Users\{user}
            var options = DbContextConnections.DbContextOptionsSqlLocalDB<AppContext>("SqlTest");

            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "SqlTest",
                IntegratedSecurity = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            await HangfireInitializer.EnsureDbDestroyedAsync(connectionString);

            using (var context = new AppContext(options))
            {
                var dbInitializer = new AppContextInitializerDropMigrate();
                await dbInitializer.InitializeAsync(context);
            }

            using (var context = new AppContext(options))
            {
                var dbInitializer = new AppContextInitializerDropCreate();
                var sql = context.GenerateCreateTablesScript();
                await dbInitializer.InitializeAsync(context);
                await context.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task HangfireInitialization()
        {
            var dbName = "HangfireSqlTest";
            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = dbName,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            await HangfireInitializer.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializer.ExistsAsync(connectionString));

            var dbInitializer = new HangfireInitializerDropCreate();
            await dbInitializer.InitializeAsync(connectionString);

            await DbInitializer.TestConnectionAsync(connectionString);

            Assert.True(await DbInitializer.ExistsAsync(connectionString));
            Assert.Equal(11, await DbInitializer.TableCountAsync(connectionString));

            await HangfireInitializer.EnsureTablesDeletedAsync(connectionString);
            Assert.Equal(0, await DbInitializer.TableCountAsync(connectionString));

            await HangfireInitializer.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializer.ExistsAsync(connectionString));
        }

        [Fact]
        public async Task MiniProfilerInitialization()
        {
            var dbName = " MiniProfilerSqlTest";
            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = dbName,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            await MiniProfilerInitializer.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializer.ExistsAsync(connectionString));

            await MiniProfilerInitializer.EnsureTablesDeletedAsync(connectionString);
            await MiniProfilerInitializer.EnsureDbAndTablesCreatedAsync(connectionString);

            await DbInitializer.TestConnectionAsync(connectionString);

            Assert.True(await DbInitializer.ExistsAsync(connectionString));
            Assert.Equal(3, await DbInitializer.TableCountAsync(connectionString));

            await MiniProfilerInitializer.EnsureTablesDeletedAsync(connectionString);
            Assert.Equal(0, await DbInitializer.TableCountAsync(connectionString));

            await MiniProfilerInitializer.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializer.ExistsAsync(connectionString));
        }
    }
}
