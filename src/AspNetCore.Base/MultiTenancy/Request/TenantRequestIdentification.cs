using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Base.MultiTenancy.Request
{
    public class TenantRequestIdentification<TTenant>
      where TTenant : AppTenant
    {
        internal readonly IServiceCollection _services;

        internal TenantRequestIdentification(IServiceCollection services)
        {
            this._services = services;
        }
    }
}
