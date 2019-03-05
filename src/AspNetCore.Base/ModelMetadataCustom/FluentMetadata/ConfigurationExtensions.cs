using AspNetCore.Base.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public static class ConfigurationExtensions
    {
        public static IMvcBuilder UseFluentMetadata(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<IAssemblyProvider, AssemblyProvider>();
            builder.Services.AddSingleton<ITypeFinder, TypeFinder>();
            builder.Services.AddSingleton<IMetadataConfiguratorProviderSingleton, MetadataConfiguratorProviderSingleton>();
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, FluentMetadataConfigureMvcOptions>();
            return builder;
        }
    }
}
