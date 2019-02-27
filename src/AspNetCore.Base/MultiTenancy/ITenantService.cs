using AspNetCore.Base.MultiTenancy.Data.Tenant;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy
{
    public interface ITenantService<TTenant> : ITenantService
        where TTenant : AppTenant
    {
        new TTenant GetTenant();
        new Task<TTenant> GetTenantByIdAsync(object key);
        void SetTenant(TTenant tenant);
    }

    public interface ITenantService
    {
        IDbContextTenantStrategy GetTenantStrategy(DbContext context);
        AppTenant GetTenant();
        Task<AppTenant> GetTenantByIdAsync(object key);
        Task SetTenantByIdAsync(object key);
        string GetTenantId();
        string TenantPropertyName { get; }
    }
}
