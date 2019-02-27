using AspNetCore.Base.Data.Helpers;
using DND.Data;
using DND.Data.Initializers;
using DND.Domain.CMS.Faqs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Data
{
    public class InMemoryDbTests
    {
        [Fact]
        public async Task InMemoryTest()
        {
            var options = DbContextConnections.DbContextOptionsInMemory<AppContext>();

            using (var context = new AppContext(options))
            {
                var uow = new AppUnitOfWork(null, null, context);

                var faq = new Faq() { Question = "test" };
                context.Faqs.Add(faq);
                await context.SaveChangesAsync();
            }

            using (var context = new AppContext(options))
            {
                var count = await context.Faqs.CountAsync();
                Assert.Equal(1, count);

                var faq = await context.Faqs.FirstOrDefaultAsync(f => f.Question == "test");
                Assert.NotNull(faq);
            }
        }

        [Fact]
        public async Task InMemoryTestInitialization()
        {
            var options = DbContextConnections.DbContextOptionsInMemory<AppContext>();

            using (var context = new AppContext(options))
            {
                var dbInitializer = new AppContextInitializerDropCreate();
                await dbInitializer.InitializeAsync(context);
                await context.Database.EnsureDeletedAsync(); //Clears Db Data
                Assert.True(context.Database.Exists());
            }
        }
    }
}