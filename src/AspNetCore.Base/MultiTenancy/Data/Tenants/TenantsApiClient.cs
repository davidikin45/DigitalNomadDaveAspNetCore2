using AspNetCore.Base.Controllers.ApiClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Data.Tenants
{
    public class TenantsApiClientBase<TTenant> : GenericApiClient<TTenant, TTenant, TTenant, TTenant>, ITenantsStore<TTenant>
        where TTenant : AppTenant
    {
        public TenantsApiClientBase(HttpClient client, JsonSerializerSettings settings)
           : base(client, settings, "tenants")
        {

        }

        public Task<List<TTenant>> GetAllTenantsAsync()
        {
            return GetAllAsync();
        }

        public Task<TTenant> GetTenantByIdAsync(object id)
        {
            return GetByIdAsync(id);
        }
    }
}
