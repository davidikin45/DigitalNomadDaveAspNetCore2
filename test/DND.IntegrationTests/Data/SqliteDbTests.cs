using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using DND.Data;
using DND.Data.Initializers;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTests.Data
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

            Assert.True(await DbInitializationHelper.ExistsAsync(connectionString));
            Assert.Equal(11, await DbInitializationHelper.TableCountAsync(connectionString));

            var fullPath = Path.GetFullPath($"{dbName}.db");
            Assert.True(File.Exists(fullPath));

            await HangfireInitializationHelper.EnsureTablesDeletedAsync(connectionString);

            Assert.Equal(0, await DbInitializationHelper.TableCountAsync(connectionString));

            Assert.True(File.Exists(fullPath));

            Assert.False(await DbInitializationHelper.HasTablesAsync(connectionString));

            await HangfireInitializationHelper.EnsureDbDestroyedAsync(connectionString);

            Assert.False(await DbInitializationHelper.ExistsAsync(connectionString));

            Assert.False(File.Exists(fullPath));
        }
    }
}
