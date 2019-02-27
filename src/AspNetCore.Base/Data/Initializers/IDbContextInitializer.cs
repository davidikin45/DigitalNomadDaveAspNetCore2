using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.Initializers
{
    public interface IDbContextInitializer<TDbContext>
        where TDbContext : DbContext
    {
        Task InitializeAsync(TDbContext context);
        void InitializeSchema(TDbContext context);
        Task InitializeDataAsync(TDbContext context, string tenantId);

        void Seed(TDbContext context, string tenantId);
    }
}
