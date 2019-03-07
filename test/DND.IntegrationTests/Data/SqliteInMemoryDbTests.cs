using AspNetCore.Base.Data;
using AspNetCore.Base.Data.Helpers;
using DND.Data;
using DND.Data.Initializers;
using DND.Domain.CMS.Faqs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTests.Data
{
    public class SqliteInMemoryDbTests
    {
        [Fact]
        public async Task SqliteInMemoryTest()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<AppContext>())
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
            using (var factory = new SqliteInMemoryDbContextFactory<AppContext>())
            {
                using (var context = await factory.CreateContextAsync())
                {
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
            using (var factory = new SqliteInMemoryDbContextFactory<AppContext>())
            {
                using (var context = await factory.CreateContextAsync(false))
                {
                    //var dbInitializer = new AppContextInitializerDropMigrate();
                    //await dbInitializer.InitializeAsync(context);
                    await context.Database.EnsureDeletedAsync(); //Clears Db Data
                    Assert.True(await context.Database.ExistsAsync());
                }
            }
        }

        [Fact]
        public async Task SqliteInMemoryTestInitializationMigrate()
        {
            using (var factory = new SqliteInMemoryDbContextFactory<AppContext>())
            {
                using (var context = await factory.CreateContextAsync(false))
                {
                    //var dbInitializer = new AppContextInitializerMigrate();
                    //await dbInitializer.InitializeAsync(context);
                    await context.Database.EnsureDeletedAsync(); //Clears Db Data
                    Assert.True(await context.Database.ExistsAsync());
                }
            }
        }
    }
}
