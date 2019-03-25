using AspNetCore.Base.Domain;
using AspNetCore.Base.MultiTenancy.Data.Tenant;
using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy
{
    public class MultiTenantService<TTenant> : ITenantService<TTenant>
        where TTenant : AppTenant
    {
        private TTenant _tenant;

        private readonly ITenantDbContextStrategyService _strategyService;
        public MultiTenantService(IHttpContextAccessor accessor, ITenantsStore<TTenant> store, ITenantIdentificationService<TTenant> service, ITenantDbContextStrategyService strategyService)
            :this(accessor, store, service)
        {
            _strategyService = strategyService;
        }

        private readonly ITenantsStore<TTenant> _store;
        public MultiTenantService(IHttpContextAccessor accessor, ITenantsStore<TTenant> store, ITenantIdentificationService<TTenant> service)
        {
            _store = store;
            _tenant = service.GetTenantAsync(accessor.HttpContext).GetAwaiter().GetResult();
        }

        public string TenantPropertyName => nameof(IEntityTenantFilter.TenantId);

        public TTenant GetTenant()
        {
            return _tenant;
        }

        public string GetTenantId()
        {
            return _tenant?.Id;
        }

        AppTenant ITenantService.GetTenant()
        {
            return _tenant;
        }

        public void SetTenant(TTenant tenant)
        {
            _tenant = tenant;
        }

        public IDbContextTenantStrategy GetTenantStrategy(DbContext context)
        {
            return _strategyService.GetStrategy(context);
        }

        public async Task<TTenant> GetTenantByIdAsync(object key)
        {
            return await _store.GetTenantByIdAsync(key);
        }

        async Task<AppTenant> ITenantService.GetTenantByIdAsync(object key)
        {
            return await _store.GetTenantByIdAsync(key);
        }

        public async Task SetTenantByIdAsync(object key)
        {
            SetTenant(await GetTenantByIdAsync(key));
        }
    }
}
