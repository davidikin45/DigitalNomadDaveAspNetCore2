﻿using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Data.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AspnetCore.Base.Data.Initializers
{
    public abstract class ContextInitializerDropCreate<TDbContext> : IDbContextInitializer<TDbContext>
        where TDbContext : DbContext
    {
        public async Task InitializeAsync(TDbContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await InitializeSchemaAsync(context, cancellationToken);
            await InitializeDataAsync(context, null, cancellationToken);
        }

        public async Task InitializeSchemaAsync(TDbContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Delete database relating to this context only
            await context.EnsureTablesAndMigrationsDeletedAsync(cancellationToken);

            //Recreate databases with the current data model. This is useful for development as no migrations are applied.
            await context.EnsureDbAndTablesCreatedAsync(cancellationToken);
        }

        public async Task InitializeDataAsync(TDbContext context, string tenantId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Seed(context, tenantId);

            await context.SaveChangesAsync(cancellationToken);

            await OnSeedCompleteAsync(context);
        }

        public abstract void Seed(TDbContext context, string tenantId);
        public virtual Task OnSeedCompleteAsync(TDbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
