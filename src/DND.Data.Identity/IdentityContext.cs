using AspNetCore.Base.Data.UnitOfWork;
using DND.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DND.Data.Identity
{
    public class IdentityContext : DbContextIdentityBase<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options = null) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //https://stackoverflow.com/questions/47767267/ef-core-2-how-to-include-roles-navigation-property-on-identityuser
            builder.Entity<User>()
               .HasMany(e => e.Roles)
               .WithOne()
               .HasForeignKey(e => e.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
        }

        public override void BuildQueries(ModelBuilder builder)
        {

        }
    }
}
