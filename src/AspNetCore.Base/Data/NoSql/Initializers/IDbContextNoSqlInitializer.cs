﻿using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.NoSql.Initializers
{
    public interface IDbContextNoSqlInitializer<TDbContext>
        where TDbContext : DbContextNoSql
    {
        Task InitializeAsync(TDbContext context, CancellationToken cancellationToken);
        Task InitializeSchemaAsync(TDbContext context, CancellationToken cancellationToken);
        Task InitializeDataAsync(TDbContext context, string tenantId, CancellationToken cancellationToken);

        void Seed(TDbContext context, string tenantId);
    }
}
