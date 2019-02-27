using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public static class CustomModelMetadataProviderExtension
    {
        public static IServiceCollection AddCustomModelMetadataProvider(this IServiceCollection services)
        {
            services.RemoveAll<IModelMetadataProvider>();
            return services.AddSingleton<IModelMetadataProvider, CustomModelMetadataProviderSingleton>();
        }
    }
}
