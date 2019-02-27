using AspNetCore.Base.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.Base.Data.Helpers
{
    public static class DbContextSaveExtensions
    {
        public static TDbContext SetTimestamps<TDbContext>(this TDbContext context) where TDbContext : DbContext
        {
            var added = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(e => e.Entity).Where(x => x is IEntityAuditable);
            var modified = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(e => e.Entity).Where(x => x is IEntityAuditable);
            var deleted = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity is IEntitySoftDelete);

            foreach (var entity in added)
            {

                ((IEntityAuditable)entity).CreatedOn = DateTime.UtcNow;

                ((IEntityAuditable)entity).UpdatedOn = DateTime.UtcNow;
            }

            foreach (var entity in modified)
            {

                ((IEntityAuditable)entity).UpdatedOn = DateTime.UtcNow;
            }

            foreach (var entityEntry in deleted)
            {
                entityEntry.State = EntityState.Modified;
                ((IEntitySoftDelete)entityEntry.Entity).IsDeleted = true;
                ((IEntitySoftDelete)entityEntry.Entity).DeletedOn = DateTime.UtcNow;
            }

            return context;
        }
    }
}
