using AspNetCore.Base.MultiTenancy;

namespace AspNetCore.Base.Settings
{
    public class TenantSettings<TTenant>
        where TTenant: AppTenant
    {
        public TTenant[] SeedTenants { get; set; }
    }
}
