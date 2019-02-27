using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        bool AutoDetectChangesEnabled { get; set; }
        QueryTrackingBehavior QueryTrackingBehavior { get; set; }

        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        DbContext DbContextByEntityType(Type entityType);

        Result<int> Complete();
        Task<Result<int>> CompleteAsync();
        Task<Result<int>> CompleteAsync(CancellationToken cancellationToken);
    }
}
