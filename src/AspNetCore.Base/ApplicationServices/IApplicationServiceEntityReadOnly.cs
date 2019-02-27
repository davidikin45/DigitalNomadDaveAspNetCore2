using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.ApplicationServices
{
    public interface IApplicationServiceEntityReadOnly<TDto> : IApplicationService
        where TDto : class
    {

        IEnumerable<TDto> GetAll(
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties);

        Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        IEnumerable<TDto> Search(
         string ownedBy = null,
         string search = "",
         Expression<Func<TDto, bool>> filter = null,
         Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
         int? pageNo = null,
         int? pageSize = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
         ;

        Task<IEnumerable<TDto>> SearchAsync(
            CancellationToken cancellationToken,
            string ownedBy = null,
            string search = "",
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        IEnumerable<TDto> Get(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetOne(
            Expression<Func<TDto, bool>> filter = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetFirst(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetById(object id, 
            bool getAggregate = false, 
            bool getAggregateAndAssociatedAggregates = false, 
            params Expression<Func<TDto, Object>>[] includeProperties);

        Task<TDto> GetByIdAsync(object id, 
             CancellationToken cancellationToken, 
             bool getAggregate = false, 
             bool getAggregateAndAssociatedAggregates = false, 
             params Expression<Func<TDto, Object>>[] includeProperties);

        TDto GetByIdWithPagedCollectionProperty(object id, 
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null);

        Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null);

        int GetByIdWithPagedCollectionPropertyCount(object id, 
            string collectionExpression,
             string search = "");

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionExpression,
            string search = "");

        IEnumerable<TDto> GetByIds(IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TDto, Object>>[] includeProperties);

        Task<IEnumerable<TDto>> GetByIdsAsync(CancellationToken cancellationToken,
         IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TDto, Object>>[] includeProperties);

        int GetCount(Expression<Func<TDto, bool>> filter = null);

        Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            );

        int GetSearchCount(string ownedBy = null, string search = "", 
            Expression<Func<TDto, bool>> filter = null);

        Task<int> GetSearchCountAsync(
          CancellationToken cancellationToken,
          string ownedBy = null,
          string search = "",
          Expression<Func<TDto, bool>> filter = null
          );

        bool Exists(Expression<Func<TDto, bool>> filter = null);

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null);

        bool Exists(object id);

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id);
    }
}
