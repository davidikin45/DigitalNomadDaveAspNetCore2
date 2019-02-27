using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Data.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.Base.Data.Initializers
{
    public abstract class ContextInitializerMigrate<TDbContext> : IDbContextInitializer<TDbContext>
         where TDbContext : DbContext
    {
        public async Task InitializeAsync(TDbContext context)
        {
            InitializeSchema(context);
            await InitializeDataAsync(context, null);
        }

        public void InitializeSchema(TDbContext context)
        {
            var script = context.Database.GenerateMigrationScript();

            //Can only be used for sqlserver and sqlite. Throws exception for InMemory
            context.Database.Migrate();
        }

        public async Task InitializeDataAsync(TDbContext context, string tenantId)
        {
            Seed(context, tenantId);

            await context.SaveChangesAsync();

            await OnSeedCompleteAsync(context);
        }

        public abstract void Seed(TDbContext context, string tenantId);
        public virtual Task OnSeedCompleteAsync(TDbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
