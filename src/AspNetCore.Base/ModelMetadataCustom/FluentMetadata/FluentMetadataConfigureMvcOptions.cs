using AspNetCore.Base.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public class FluentMetadataConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IMetadataConfiguratorProviderSingleton _provider;

        public FluentMetadataConfigureMvcOptions(IMetadataConfiguratorProviderSingleton provider)
        {
            _provider = provider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Insert(0, new FluentMetadataProvider(_provider));
        }
    }
}