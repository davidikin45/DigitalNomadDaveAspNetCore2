using AspNetCore.Base.Domain;

namespace AspNetCore.Base.Data.Repository
{
    public interface IGenericRepository<TEntity> : IGenericReadOnlyRepository<TEntity>
      where TEntity : class
    {
        TEntity Add(TEntity entity, string addedBy);
        TEntity Update(TEntity entity, string updatedBy);
        void Delete(object id, string deletedBy);
        void SoftDelete(IEntitySoftDelete entity, string deletedBy);
        void Delete(TEntity entity, string deletedBy);
    }
}
