using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public static class ConfigurationExtensions
    {
        public static IMvcBuilder UseDisplayAttributes(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, DisplayAttributesConfigureMvcOptions>();
            return builder;
        }

        public static IMvcBuilder UseDisplayConventionFilters(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, DisplayConventionFiltersConfigureMvcOptions>();
            return builder;
        }
    }
}
