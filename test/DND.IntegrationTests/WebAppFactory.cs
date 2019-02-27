using AspNetCore.Testing.TestServer;
using DND.Web;
using Microsoft.Extensions.DependencyInjection;

namespace DND.IntegrationTests
{
    public class WebAppFactory : WebApplicationFactoryBase<Startup>
    {
        protected override Microsoft.AspNetCore.Hosting.IWebHostBuilder CreateWebHostBuilder()
        {
            var args = new string[] { };
            var contentRoot = GetContentRoot();
            var config = Program.BuildWebHostConfiguration("Integration", contentRoot);

            Program.Configuration = config;
            var builder = Program.CreateWebHostBuilder(args);

            return builder;
        }

        public override void ConfigureTestServices(IServiceCollection services)
        {

        }
    }
}
