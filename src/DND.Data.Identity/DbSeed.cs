using AspNetCore.Base;
using AspNetCore.Base.Data;
using AspNetCore.Base.Data.UnitOfWork;
using DND.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DND.Data.Identity
{
    public class DbSeed : DbSeedIdentity<User>
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        public DbSeed(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public override IEnumerable<SeedRole> GetRolePermissions(DbContextIdentityBase<User> context)
        {
            //To allow anonymous need to add method here AND add AllowAnonymousAttribute
            return new List<SeedRole>()
            {
               new SeedRole(Role.anonymous.ToString(),
               ResourceCollectionsCore.Auth.Scopes.Authenticate,
               ResourceCollectionsCore.Auth.Scopes.ForgotPassword,
               ResourceCollectionsCore.Auth.Scopes.ResetPassword,
               ResourceCollectionsCore.Admin.Scopes.Read
               ),
               new SeedRole(Role.read_only.ToString(),
               ResourceCollectionsCore.Admin.Scopes.Read
               ),
               new SeedRole(Role.admin.ToString(),
               ResourceCollectionsCore.Admin.Scopes.Full
               )
            };
        }

        public override IEnumerable<SeedUser> GetUserRoles(DbContextIdentityBase<User> context)
        {
            return new List<SeedUser>()
            {
                 new SeedUser(User.CreateUser(_passwordHasher, "admin", "admin@admin.com", "password"), true, Role.admin.ToString())
            };
        }
    }
}
