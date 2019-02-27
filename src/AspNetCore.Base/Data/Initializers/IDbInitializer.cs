using System.Threading.Tasks;

namespace AspNetCore.Base.Data.Initializers
{
    public interface IDbInitializer
    {
        Task InitializeAsync(string connectionString);
        void InitializeSchema(string connectionString);
        Task InitializeDataAsync(string context, string tenantId);

        void Seed(string connectionString, string tenantId);
    }
}
