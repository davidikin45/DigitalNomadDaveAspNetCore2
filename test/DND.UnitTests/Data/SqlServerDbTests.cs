using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using DND.Data;
using DND.Data.Initializers;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Data
{
    public class SqlServerDbTests
    {
        [Fact]
        public async Task Initialization()
        {
            //db location = C:\Users\{user}
            var options = DbContextConnections.DbContextOptionsSqlLocalDB<AppContext>("SqlTest");

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
            
            HangfireInitializationHelper.EnsureDbDestroyed(connectionString);
            Assert.False(DbInitializationHelper.Exists(connectionString));

            var dbInitializer = new HangfireInitializerDropCreate();
            await dbInitializer.InitializeAsync(connectionString);

            DbInitializationHelper.TestConnection(connectionString);

            Assert.True(DbInitializationHelper.Exists(connectionString));
            Assert.Equal(11, DbInitializationHelper.TableCount(connectionString));

            HangfireInitializationHelper.EnsureTablesDeleted(connectionString);
            Assert.Equal(0, DbInitializationHelper.TableCount(connectionString));

            HangfireInitializationHelper.EnsureDbDestroyed(connectionString);
            Assert.False(DbInitializationHelper.Exists(connectionString));
        }
    }
}
