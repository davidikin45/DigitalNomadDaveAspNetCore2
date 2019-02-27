using AspNetCore.Base.Data.Initializers;
using System.Threading.Tasks;

namespace AspNetCore.Base.Hangfire
{
    public class HangfireInitializerDropCreate : IDbInitializer
    {
        public async Task InitializeAsync(string connectionString)
        {
            InitializeSchema(connectionString);
            await InitializeDataAsync(connectionString, null);
        }

        public void InitializeSchema(string connectionString)
        {
            HangfireInitializationHelper.EnsureTablesDeleted(connectionString);
            HangfireInitializationHelper.EnsureDbAndTablesCreated(connectionString);           
        }

        public async Task InitializeDataAsync(string connectionString, string tenantId)
        {
            Seed(connectionString, tenantId);

            await OnSeedCompleteAsync(connectionString);
        }

        public virtual void Seed(string connectionString, string tenantId)
        {

        }

        public virtual Task OnSeedCompleteAsync(string connectionString)
        {
            return Task.CompletedTask;
        }
    }
}
