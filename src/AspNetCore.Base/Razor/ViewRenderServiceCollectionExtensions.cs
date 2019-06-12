using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Base.Razor
{
    public static class ViewRenderServiceCollectionExtensions
    {
        public static IServiceCollection AddViewRenderer(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services.AddSingleton<IViewRenderService, ViewRenderService>();
        }
    }
}
