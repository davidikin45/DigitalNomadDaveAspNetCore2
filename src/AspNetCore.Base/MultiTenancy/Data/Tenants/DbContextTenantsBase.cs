using AspNetCore.Base.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Data.Tenants
{
    public abstract class DbContextTenantsBase<TTenant> : DbContextBase, ITenantsStore<TTenant>
        where TTenant : AppTenant
    {
        public DbSet<TTenant> Tenants { get; set; }

        public DbContextTenantsBase(DbContextOptions options)
            : base(options)
        {

        }

        public async Task<List<TTenant>> GetAllTenantsAsync()
        {
            return await this.Tenants.ToListAsync();
        }

        public async Task<TTenant> GetTenantByIdAsync(object id)
        {
            return await this.Tenants.FindAsync(id);
        }
    }
}
