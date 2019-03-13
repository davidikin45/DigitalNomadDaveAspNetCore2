using AspNetCore.Base.Hangfire;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DND.Data
{
    public class HangfireDbInitializer : IAsyncDbInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ConnectionStrings _connectionStrings;

        public int Order => 1;

        public HangfireDbInitializer(ConnectionStrings connectionStrings, IHostingEnvironment hostingEnvironment)
        {
            _connectionStrings = connectionStrings;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
            {
                var dbInitializer = new HangfireInitializerCreate();
                await dbInitializer.InitializeAsync(_connectionStrings["HangfireConnection"]);
            }
            else
            {
                var dbInitializer = new HangfireInitializerDropCreate();
                await dbInitializer.InitializeAsync(_connectionStrings["HangfireConnection"]);
            }
        }
    }
}
