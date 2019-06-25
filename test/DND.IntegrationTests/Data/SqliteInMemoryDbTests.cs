using AspNetCore.Base.Data;
using DND.Data.Initializers;
using DND.Domain.CMS.Faqs;
using Hangfire;
using Hangfire.Initialization;
using Microsoft.EntityFrameworkCore;
using MiniProfiler.Initialization;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DND.IntegrationTests.Data
{
    public class SqliteInMemoryDbTests
    {
        private readonly ITestOutputHelper _output;

        public SqliteInMemoryDbTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SqliteInMemoryTest()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<DND.Data.AppContext>(log => _output.WriteLine(log)))
            {
                using (var context = await factory.CreateContextAsync())
                {
                    var faq = new Faq() { Question = "test", Answer = "answer" };
                    context.Faqs.Add(faq);
                    await context.SaveChangesAsync();
                }

                using (var context = await factory.CreateContextAsync())
                {
                    var count = await context.Faqs.CountAsync();
                    Assert.Equal(1, count);

                    var faq = await context.Faqs.FirstOrDefaultAsync(f => f.Question == "test");
                    Assert.NotNull(faq);
                }
            }
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationDropCreate()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<DND.Data.AppContext>(log => _output.WriteLine(log)))
            {
                using (var context = await factory.CreateContextAsync())
                {
                    await DbContextInitializationExtensions.EnsureTablesAndMigrationsDeletedAsync(context);
                    await context.Database.EnsureCreatedAsync();
            
                    await DbContextInitializationExtensions.EnsureDbAndTablesCreatedAsync(context);
                    var dbInitializer = new AppContextInitializerDropCreate();
                    await dbInitializer.InitializeAsync(context);
                    await context.Database.EnsureDeletedAsync(); //Clears Db Data
                    Assert.True(await context.Database.ExistsAsync());
                }
            }
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationDropMigrate()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<DND.Data.AppContext>(log => _output.WriteLine(log)))
            {
                using (var context = await factory.CreateContextAsync(false))
                {
                    var dbInitializer = new AppContextInitializerDropMigrate();
                    await dbInitializer.InitializeAsync(context);
                    await context.Database.EnsureDeletedAsync(); //Clears Db Data
                    Assert.True(await context.Database.ExistsAsync());
                }
            }
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationMigrate()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<DND.Data.AppContext>(log => _output.WriteLine(log)))
            {
                using (var context = await factory.CreateContextAsync(false))
                {
                    var dbInitializer = new AppContextInitializerMigrate();
                    await dbInitializer.InitializeAsync(context);
                    await context.Database.EnsureDeletedAsync(); //Clears Db Data
                    Assert.True(await context.Database.ExistsAsync());
                }
            }
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationHangfire()
        {
            using (var factory = new SqliteInMemoryConnectionFactory())
            {
                var connection = await factory.GetConnection();

                //Initialise Database
                await HangfireInitializer.EnsureDbDestroyedAsync(connection);
                await HangfireInitializer.EnsureDbAndTablesCreatedAsync(connection);
                
                //Start Hangfire
                var hangfire = HangfireLauncher.StartHangfireServer("test-server", connection, false);


                hangfire.BackgroundJobClient.Enqueue(() => Console.WriteLine("Test"));
            }
        }

        [Fact]
        public void SqliteInMemoryTestInitializationHangfire2()
        {
            //Start Hangfire
            var hangfire = HangfireLauncher.StartHangfireServerSQLiteInMemory("test-server");
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationMiniProfiler()
        {
            using (var factory = new SqliteInMemoryConnectionFactory())
            {
                var connection = await factory.GetConnection();
                await MiniProfilerInitializer.EnsureDbDestroyedAsync(connection);
                await MiniProfilerInitializer.EnsureDbAndTablesCreatedAsync(connection);
            }
        }
    }
}
