using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AspNetCore.Base
{
    public class Config
    {
        private static readonly Dictionary<string, string> InMemoryDefaults = new Dictionary<string, string> { {
                WebHostDefaults.EnvironmentKey, "Development"
            } };

        public static IConfigurationRoot Build(string[] args, string contentRoot, string assemblyName)
        {
            var configEnvironmentBuilder = new ConfigurationBuilder()
                   .AddInMemoryCollection(InMemoryDefaults)
                   .AddEnvironmentVariables(prefix: "ASPNETCORE_");

            if (args != null)
            {
                configEnvironmentBuilder.AddCommandLine(args);
            }
            var configEnvironment = configEnvironmentBuilder.Build();

            var appSettingsFileName = "appsettings.json";
            var appSettingsEnvironmentFilename = "appsettings." + (configEnvironment[WebHostDefaults.EnvironmentKey] ?? "Production") + ".json";

            Console.WriteLine($"Loading Settings:" + Environment.NewLine +
                               $"{contentRoot}\\{appSettingsFileName}" + Environment.NewLine +
                               $"{contentRoot}\\{appSettingsEnvironmentFilename}");

            var settingSuffix = assemblyName.ToUpperInvariant().Replace(".", "_");
            var settingName = $"TEST_CONTENTROOT_{settingSuffix}";
            Dictionary<string, string> InMemoryDefaultsContentRoot = new Dictionary<string, string> { {
                settingName, contentRoot
            } };

            var config = new ConfigurationBuilder()
           .AddInMemoryCollection(InMemoryDefaults)
           .AddInMemoryCollection(InMemoryDefaultsContentRoot)
           .SetBasePath(contentRoot)
           .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
           .AddJsonFile(appSettingsEnvironmentFilename, optional: true, reloadOnChange: true)
           .AddEnvironmentVariables(prefix: "ASPNETCORE_");

            if (args != null)
            {
                config.AddCommandLine(args);
            }

            return config.Build();
        }
    }
}