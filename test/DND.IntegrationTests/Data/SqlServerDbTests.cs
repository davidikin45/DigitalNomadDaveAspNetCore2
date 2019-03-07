using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using DND.Data;
using DND.Data.Initializers;
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

            await HangfireInitializationHelper.EnsureDbDestroyedAsync(connectionString);

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

            await HangfireInitializationHelper.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializationHelper.ExistsAsync(connectionString));

            var dbInitializer = new HangfireInitializerDropCreate();
            await dbInitializer.InitializeAsync(connectionString);

            await DbInitializationHelper.TestConnectionAsync(connectionString);

            Assert.True(await DbInitializationHelper.ExistsAsync(connectionString));
            Assert.Equal(11, await DbInitializationHelper.TableCountAsync(connectionString));

            await HangfireInitializationHelper.EnsureTablesDeletedAsync(connectionString);
            Assert.Equal(0, await DbInitializationHelper.TableCountAsync(connectionString));

            await HangfireInitializationHelper.EnsureDbDestroyedAsync(connectionString);
            Assert.False(await DbInitializationHelper.ExistsAsync(connectionString));
        }
    }
}