using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Base.Localization
{
    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
            app.UseMiddleware<RedirectUnsupportedCulturesMiddleware>(new object[] { false });
        }
    }
}
