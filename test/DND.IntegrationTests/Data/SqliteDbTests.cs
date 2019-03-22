using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Hangfire;
using Database.Initialization;
using DND.Data;
using DND.Data.Initializers;
using Hangfire.Initialization;
using Microsoft.EntityFrameworkCore;
using MiniProfiler.Initialization;
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

            Assert.True(await DbInitializer.ExistsAsync(connectionString));
            Assert.Equal(11, await DbInitializer.TableCountAsync(connectionString));

            var fullPath = Path.GetFullPath($"{dbName}.db");
            Assert.True(File.Exists(fullPath));

            await HangfireInitializer.EnsureTablesDeletedAsync(connectionString);


            Assert.Equal(0, await DbInitializer.TableCountAsync(connectionString));

            Assert.True(File.Exists(fullPath));

            Assert.False(await DbInitializer.HasTablesAsync(connectionString));

            await HangfireInitializer.EnsureDbDestroyedAsync(connectionString);

            Assert.False(await DbInitializer.ExistsAsync(connectionString));

            Assert.False(File.Exists(fullPath));
        }

        [Fact]
        public async Task MiniProfilerInitialization()
        {
            var dbName = "MiniProfilerSqliteTest";
            var connectionString = $"Data Source={dbName}.db;";

            await MiniProfilerInitializer.EnsureTablesDeletedAsync(connectionString);
            await MiniProfilerInitializer.EnsureDbAndTablesCreatedAsync(connectionString);

            Assert.True(await DbInitializer.ExistsAsync(connectionString));
            Assert.Equal(11, await DbInitializer.TableCountAsync(connectionString));

            var fullPath = Path.GetFullPath($"{dbName}.db");
            Assert.True(File.Exists(fullPath));

            await MiniProfilerInitializer.EnsureTablesDeletedAsync(connectionString);

            Assert.Equal(0, await DbInitializer.TableCountAsync(connectionString));

            Assert.True(File.Exists(fullPath));

            Assert.False(await DbInitializer.HasTablesAsync(connectionString));

            await MiniProfilerInitializer.EnsureDbDestroyedAsync(connectionString);

            Assert.False(await DbInitializer.ExistsAsync(connectionString));

            Assert.False(File.Exists(fullPath));
        }
    }
}
