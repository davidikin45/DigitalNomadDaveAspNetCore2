using AspnetCore.Base.Data.Initializers;
using DND.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace DND.Data.Identity.Initializers
{
    public class IdentityContextInitializerMigrate : ContextInitializerMigrate<IdentityContext>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        public IdentityContextInitializerMigrate(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public IdentityContextInitializerMigrate()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public override void Seed(IdentityContext context, string tenantId)
        {
            var dbSeeder = new DbSeed(_passwordHasher);
            dbSeeder.SeedData(context);
        }
    }
}
