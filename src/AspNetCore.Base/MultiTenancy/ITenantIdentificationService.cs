using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy
{
    public interface ITenantIdentificationService<TTenant> : ITenantIdentificationStrategy
        where TTenant : AppTenant

    {
        Task<TTenant> GetTenantAsync(HttpContext httpContext);
    }
}
