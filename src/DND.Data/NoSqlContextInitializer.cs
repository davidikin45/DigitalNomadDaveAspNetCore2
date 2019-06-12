using AspNetCore.Base.Extensions;
using AspNetCore.Base.Tasks;
using DND.Data.Initializers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DND.Data
{
    public class NoSqlContextInitializer : IAsyncDbInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly NoSqlContext _context;

        public int Order => 0;

        public NoSqlContextInitializer(NoSqlContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
            {
                var dbInitializer = new NoSqlContextInitializerMigrate();
                await dbInitializer.InitializeAsync(_context);
            }
            else if (_hostingEnvironment.IsIntegration())
            {
                var dbInitializer = new NoSqlContextInitializerDropCreate();
                await dbInitializer.InitializeAsync(_context);
            }
            else
            {
                var dbInitializer = new NoSqlContextInitializerDropCreate();
                await dbInitializer.InitializeAsync(_context);
            }
        }
    }
}
