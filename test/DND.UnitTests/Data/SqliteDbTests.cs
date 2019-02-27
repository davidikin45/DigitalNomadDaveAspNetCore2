using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using DND.Data;
using DND.Data.Initializers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Data
{
    public class SqliteDbTests
    {
        [Fact]
        public async Task Initialization()
        {
            var dbName = "SqliteTest";
            var options = DbContextConnections.DbContextOptionsSqlite<AppContext>(dbName);

            using (var context = new AppContext(options))
            {
                var dbInitializer = new AppContextInitializerDropMigrate();
                await dbInitializer.InitializeAsync(context);
            }

            using (var context = new AppContext(options))
            {
                var dbInitializer = new AppContextInitializerDropCreate();
                await dbInitializer.InitializeAsync(context);
                await context.Database.EnsureDeletedAsync();

                var fullPath = Path.GetFullPath($"{dbName}.db");
                Assert.False(File.Exists(fullPath));
            }
        }

        [Fact]
        public async Task HangfireInitialization()
        {
            var dbName = "HangfireSqliteTest";
            var connectionString = $"Data Source={dbName}.db;";

            var dbInitializer = new HangfireInitializerDropCreate();
            await dbInitializer.InitializeAsync(connectionString);

            Assert.True(DbInitializationHelper.Exists(connectionString));
            Assert.Equal(11, DbInitializationHelper.TableCount(connectionString));

            var fullPath = Path.GetFullPath($"{dbName}.db");
            Assert.True(File.Exists(fullPath));

            HangfireInitializationHelper.EnsureTablesDeleted(connectionString);

            Assert.Equal(0, DbInitializationHelper.TableCount(connectionString));

            Assert.True(File.Exists(fullPath));

            Assert.False(DbInitializationHelper.HasTables(connectionString));

            HangfireInitializationHelper.EnsureDbDestroyed(connectionString);

            Assert.False(DbInitializationHelper.Exists(connectionString));

            Assert.False(File.Exists(fullPath));
        }
    }
}
