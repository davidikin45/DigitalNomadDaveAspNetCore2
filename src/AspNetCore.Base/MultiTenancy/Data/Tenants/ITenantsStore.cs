using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Data.Tenants
{
    public interface ITenantsStore<TTenant>
        where TTenant : AppTenant
    {
        Task<List<TTenant>> GetAllTenantsAsync();
        Task<TTenant> GetTenantByIdAsync(object id);
    }
}
