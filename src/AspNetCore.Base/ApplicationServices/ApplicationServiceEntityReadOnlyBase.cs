using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Data.UnitOfWork;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.ApplicationServices
{
    public abstract class ApplicationServiceEntityReadOnlyBase<TEntity, TDto, TUnitOfWork> : ApplicationServiceBase, IApplicationServiceEntityReadOnly<TDto>
          where TEntity : class
          where TDto : class
          where TUnitOfWork : IUnitOfWork
    {
        protected virtual TUnitOfWork UnitOfWork { get; }
        protected virtual IGenericRepository<TEntity> Repository => UnitOfWork.Repository<TEntity>();

        public ApplicationServiceEntityReadOnlyBase(TUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService)
           : base(mapper, authorizationService, userService, validationService)
        {
            UnitOfWork = unitOfWork;
        }

        public virtual void AddIncludes(List<Expression<Func<TEntity, Object>>> includes)
        {

        }

        public virtual bool GetAggregate => false;
        public virtual bool GetAggregateAndAssociatedAggregates => false;

        #region GetAll
        public virtual IEnumerable<TDto> GetAll(
        Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
        int? pageNo = null,
        int? pageSize = null,
        bool getAggregate = false,
        bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read).Wait();

            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.GetAll(orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read);

            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.GetAllAsync(cancellationToken, orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }
        #endregion

        #region Search
        public virtual IEnumerable<TDto> Search(
       string ownedBy = null,
       string search = "",
       Expression<Func<TDto, bool>> filter = null,
       Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
       int? pageNo = null,
       int? pageSize = null,
       bool getAggregate = false,
       bool getAggregateAndAssociatedAggregates = false,
       params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.Search(ownedBy, search, filterConverted, orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            var entities = entityList.ToList();

            foreach (var entity in entities)
            {
                AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            }

            IEnumerable<TDto> dtoList = entities.Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> SearchAsync(
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
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.SearchAsync(cancellationToken, ownedBy, search, filterConverted, orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            var entities = entityList.ToList();

            foreach (var entity in entities)
            {
               await AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);
            }

            IEnumerable<TDto> dtoList = entityList.Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetSearchCount(
        string ownedBy = null,
        string search = "",
       Expression<Func<TDto, bool>> filter = null)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.GetSearchCount(ownedBy, search, filterConverted);
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string ownedBy = null,
             string search = "",
            Expression<Func<TDto, bool>> filter = null)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.GetSearchCountAsync(cancellationToken, ownedBy, search, filterConverted).ConfigureAwait(false);
        }
        #endregion

        #region Get
        public virtual IEnumerable<TDto> Get(
           Expression<Func<TDto, bool>> filter = null,
           Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
           int? pageNo = null,
           int? pageSize = null,
           bool getAggregate = false,
           bool getAggregateAndAssociatedAggregates = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = Repository.Get(filterConverted, orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            var entities = entityList.ToList();

            foreach (var entity in entities)
            {
                AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            }

            IEnumerable<TDto> dtoList = entities.Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await Repository.GetAsync(cancellationToken, filterConverted, orderByConverted, pageNo, pageSize, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            var entities = entityList.ToList();

            foreach (var entity in entities)
            {
                await AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);
            }

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetCount(
        Expression<Func<TDto, bool>> filter = null)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.GetCount(filterConverted);
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.GetCountAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        #endregion

        #region GetOne
        public virtual TDto GetOne(
          Expression<Func<TDto, bool>> filter = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetOne(filterConverted, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetOneAsync(cancellationToken, filterConverted, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            await AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetFirst
        public virtual TDto GetFirst(
         Expression<Func<TDto, bool>> filter = null,
         Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetFirst(filterConverted, orderByConverted, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetFirstAsync(cancellationToken, filterConverted, orderByConverted, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            await AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetById
        public virtual TDto GetById(object id,
           bool getAggregate = false,
           bool getAggregateAndAssociatedAggregates = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = Repository.GetById(id, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken),
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await Repository.GetByIdAsync(cancellationToken, id, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            await AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TDto GetByIdWithPagedCollectionProperty(object id,
           string collectionExpression,
           string search = "",
           string orderBy = null,
           bool ascending = false,
           int? pageNo = null,
           int? pageSize = null)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            var bo = Repository.GetByIdWithPagedCollectionProperty(id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);

            AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null,
            int? pageSize = null)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var bo = await Repository.GetByIdWithPagedCollectionPropertyAsync(cancellationToken, id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);

            await AuthorizeResourceOperationAsync(bo, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            return Mapper.Map<TDto>(bo);
        }

        public int GetByIdWithPagedCollectionPropertyCount(object id,
            string collectionExpression,
            string search = "")
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            //var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return Repository.GetByIdWithPagedCollectionPropertyCount(id, collectionExpression, search);
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "")
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            // var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return await Repository.GetByIdWithPagedCollectionPropertyCountAsync(cancellationToken, id, collectionExpression, search);
        }
        #endregion

        #region GetByIds
        public virtual IEnumerable<TDto> GetByIds(IEnumerable<object> ids,
        bool getAggregate = false,
        bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = Repository.GetByIds(ids, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted);

            var entities = result.ToList();

            foreach (var entity in entities)
            {
                AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            }

            return Mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<IEnumerable<TDto>> GetByIdsAsync(CancellationToken cancellationToken,
         IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = await Repository.GetByIdsAsync(cancellationToken, ids, GetAggregate || getAggregate, GetAggregateAndAssociatedAggregates || getAggregateAndAssociatedAggregates, includesConverted).ConfigureAwait(false);

            var entities = result.ToList();

            foreach (var entity in entities)
            {
                AuthorizeResourceOperationAsync(entity, ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            }

            return Mapper.Map<IEnumerable<TDto>>(entities);
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TDto, bool>> filter = null)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return Repository.Exists(filterConverted);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await Repository.ExistsAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        public virtual bool Exists(object id)
        {
            AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner).Wait();
            return Repository.Exists(id);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            await AuthorizeResourceOperationAsync(ResourceCollectionsCore.CRUD.Operations.Read, ResourceCollectionsCore.CRUD.Operations.ReadOwner);

            return await Repository.ExistsAsync(cancellationToken, id).ConfigureAwait(false);
        }
        #endregion
    }
}
