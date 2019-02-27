using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace AspNetCore.Base.MultiTenancy
{
    public static class TenantJsonExtensions
    {
        public static IConfigurationBuilder AddTenantJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return builder.AddTenantJsonFile(s =>
            {
                s.FileProvider = null;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }

        public static IConfigurationBuilder AddTenantJsonFile(this IConfigurationBuilder builder, Action<TenantJsonConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }

    public static class TenantConfig
    {
        public static IConfiguration BuildTenantConfiguration(IHostingEnvironment environment, string tenantId)
        {
            var appSettingsFileName = $"appsettings.{tenantId}.json";
            var appSettingsEnvironmentFilename = $"appsettings.{tenantId}.{environment.EnvironmentName}.json";

            var config = new ConfigurationBuilder()
                      .SetBasePath(environment.ContentRootPath)
                      .AddTenantJsonFile(appSettingsFileName, optional: true, reloadOnChange: true)
                      .AddTenantJsonFile(appSettingsEnvironmentFilename, optional: true, reloadOnChange: true);

            return config.Build();
        }
    }
}
