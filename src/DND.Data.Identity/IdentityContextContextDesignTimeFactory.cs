using AspNetCore.Base.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DND.Data.Identity
{
    public class IdentityContextContextDesignTimeFactory : DesignTimeDbContextFactoryBase<IdentityContext>
    {
        public IdentityContextContextDesignTimeFactory()
            : base("IdentityConnection", typeof(IdentityContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override IdentityContext CreateNewInstance(DbContextOptions<IdentityContext> options)
        {
            return new IdentityContext(options);
        }
    }
}
