using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public sealed class TenantDbContextIdentification<TDbContext>
        where TDbContext : DbContext
    {
        internal readonly IServiceCollection _services;

        internal TenantDbContextIdentification(IServiceCollection services)
        {
            this._services = services;
        }
    }
}
