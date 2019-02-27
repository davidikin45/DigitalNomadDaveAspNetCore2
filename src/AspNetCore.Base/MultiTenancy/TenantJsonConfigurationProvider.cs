using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace AspNetCore.Base.MultiTenancy
{
    public sealed class TenantJsonConfigurationProvider : JsonConfigurationProvider
    {
        public TenantJsonConfigurationProvider(TenantJsonConfigurationSource config) : base(config)
        {
        }

        public override void Load()
        {
            base.Load();
        }
    }

    public class TenantJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new TenantJsonConfigurationProvider(this);
        }
    }
}
