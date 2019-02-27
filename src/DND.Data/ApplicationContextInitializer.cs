using AspNetCore.Base.Extensions;
using AspNetCore.Base.Tasks;
using DND.Data.Initializers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DND.Data
{
    public class ApplicationContextInitializer : IAsyncDbInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppContext _context;

        public int Order => 0;

        public ApplicationContextInitializer(AppContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
            {
                var dbInitializer = new AppContextInitializerMigrate();
                await dbInitializer.InitializeAsync(_context);
            }
            else if (_hostingEnvironment.IsIntegration())
            {
                var dbInitializer = new AppContextInitializerDropMigrate();
                await dbInitializer.InitializeAsync(_context);
            }
            else
            {
                var dbInitializer = new AppContextInitializerDropMigrate();
                await dbInitializer.InitializeAsync(_context);
            }
        }
    }
}
