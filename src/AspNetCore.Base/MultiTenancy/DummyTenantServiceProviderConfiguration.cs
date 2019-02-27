using Autofac.Multitenant;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Base.MultiTenancy
{
    internal sealed class DummyTenantServiceProviderConfiguration : ITenantConfiguration
    {
        internal static readonly ITenantConfiguration Instance = new DummyTenantServiceProviderConfiguration();

        private DummyTenantServiceProviderConfiguration()
        {
        }

        public string TenantId => string.Empty;

        public void Configure(IConfiguration configuration)
        {

        }

        public void ConfigureServices(ConfigurationActionBuilder services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
           
        }

        public void ConfigureHangfireJobs(IRecurringJobManager recurringJobManager, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {

        }
    }
}
