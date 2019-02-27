using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.MultiTenancy.Data.Tenants
{
    public abstract class DbSeedTenants<TTenant> where TTenant : AppTenant
    {
        public abstract IEnumerable<TTenant> InMemorySeedTenants { get; }

        public virtual void Seed(DbContextTenantsBase<TTenant> context, IEnumerable<TTenant> tenants)
        {
            SeedTenants(context, tenants.Concat(InMemorySeedTenants));
        }

        public virtual void Seed(DbContextTenantsBase<TTenant> context)
        {
            SeedTenants(context, InMemorySeedTenants);
        }

        private void SeedTenants(DbContextTenantsBase<TTenant> context, IEnumerable<TTenant> tenants)
        {
            foreach (var tenant in InMemorySeedTenants.Concat(tenants))
            {
                var dbTenant = context.Tenants.Find(tenant.Id);

                if (dbTenant == null)
                {
                    context.Tenants.Add(tenant);
                }
                else
                {
                    context.Tenants.Update(tenant);
                }
            }
        }
    }
}
