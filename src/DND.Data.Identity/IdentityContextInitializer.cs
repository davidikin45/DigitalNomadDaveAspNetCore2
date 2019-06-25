using AspNetCore.Base.Extensions;
using AspNetCore.Base.Tasks;
using AspNetCore.Mvc.Extensions;
using DND.Data.Identity.Initializers;
using DND.Domain.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DND.Data.Identity
{
    public class IdentityContextInitializer : IAsyncDbInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IdentityContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public int Order => 0;

        public IdentityContextInitializer(IdentityContext context, IPasswordHasher<User> passwordHasher, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
            {
                var migrationInitializer = new IdentityContextInitializerMigrate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
            else if (_hostingEnvironment.IsIntegration())
            {
                var migrationInitializer = new IdentityContextInitializerDropMigrate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
            else
            {
                var migrationInitializer = new IdentityContextInitializerDropCreate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
        }
    }
}
