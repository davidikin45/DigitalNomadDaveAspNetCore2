using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System.Linq;

namespace AspNetCore.Base.Azure
{
    //https://joonasw.net/view/azure-ad-managed-service-identity
    //https://joonasw.net/view/aspnet-core-azure-keyvault-msi
    public static class AzureKeyVaultExtensions
    {
        public static IWebHostBuilder UseAzureKeyVault(this IWebHostBuilder webHostBuilder, string vaultName = null)
        {
            return webHostBuilder.ConfigureAppConfiguration((ctx, builder) =>
            {
                var config = builder.Build();

                if(vaultName == null)
                {
                    var keyVaultSettings = GetKeyVaultSettings(config);
                    if(keyVaultSettings != null)
                    {
                        vaultName = keyVaultSettings.Name;
                    }
                }

                if (!string.IsNullOrWhiteSpace(vaultName) && !ctx.HostingEnvironment.IsDevelopment())
                    {
                        //Section--Name
                        var tokenProvider = new AzureServiceTokenProvider();
                        var kvClient = new KeyVaultClient((authority, resource, scope) => tokenProvider.KeyVaultTokenCallback(authority, resource, scope));
                        builder.AddAzureKeyVault($"https://{vaultName}.vault.azure.net", kvClient, new DefaultKeyVaultSecretManager());
                    }
                });
        }

        private static KeyVaultSettings GetKeyVaultSettings(IConfigurationRoot config)
        {
            var sectionKey = "KeyVaultSettings";
            var hasKeyVaultSettings = config.GetChildren().Any(item => item.Key == sectionKey);

            if (!hasKeyVaultSettings)
                return null;

            var settingsSection = config.GetSection(sectionKey);
            return settingsSection.Get<KeyVaultSettings>();
        }
    }
}
