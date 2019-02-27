using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Domain;
using AspNetCore.Base.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Data.Repository
{
    public class GenericReadOnlyRepository<TEntity> : IGenericReadOnlyRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext context;

        //AsNoTracking() causes EF to bypass cache for reads and writes - Ideal for Web Applications and short lived contexts
        public GenericReadOnlyRepository(DbContext context)
        {
            this.context = context;
        }

        protected virtual IQueryable<TEntity> GetQueryable(
            bool tracking,
            string ownedBy = null,
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            //includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = context.Set<TEntity>();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            else
            {
                query = query.AsTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (ownedBy != null && typeof(IEntityOwned).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where((e) => ((IEntityOwned)e).OwnedBy == ownedBy || ((IEntityOwned)e).OwnedBy == null);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = CreateSearchQuery(query, search);
            }

            if (getAggregateAndAssociatedAggregates)
            {
                var includesList = GetAllCompositionAndAggregationRelationshipPropertyIncludes(false, typeof(TEntity));

                foreach (var include in includesList)
                {
                    query = query.Include(include);
                }
            }
            else
            {
                if (getAggregate)
                {
                    //For Aggregate Roots
                    var includesList = GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, typeof(TEntity));

                    foreach (var include in includesList)
                    {
                        query = query.Include(include);
                    }
                }

                if (includeProperties != null && includeProperties.Count() > 0)
                {
                    foreach (var includeExpression in includeProperties)
                    {
                        query = query.Include(includeExpression);
                    }
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            DebugSQL(query);

            return query;
        }

        private List<string> GetAllCompositionRelationshipPropertyIncludes(Type type, int maxDepth = 10)
        {
            return GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, type, null, 0, maxDepth);
        }

        private List<string> GetAllCompositionAndAggregationRelationshipPropertyIncludes(bool compositionRelationshipsOnly, Type type, string path = null, int depth = 0, int maxDepth = 5)
        {
            List<string> includesList = new List<string>();
            if (depth > maxDepth)
            {
                return includesList;
            }

            List<Type> excludeTypes = new List<Type>()
            {
                typeof(DateTime),
                typeof(String),
                typeof(byte[])
           };

            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => p.CanWrite && !p.PropertyType.IsValueType && !excludeTypes.Contains(p.PropertyType) && ((!compositionRelationshipsOnly && !p.PropertyType.IsCollection()) || (p.PropertyType.IsCollection() && type != p.PropertyType.GetGenericArguments().First()))).ToList();

            foreach (var p in properties)
            {
                var includePath = !string.IsNullOrWhiteSpace(path) ? path + "." + p.Name : p.Name;

                includesList.Add(includePath);

                Type propType = null;
                if (p.PropertyType.IsCollection())
                {
                    propType = type.GetGenericArguments(p.Name).First();
                }
                else
                {
                    propType = p.PropertyType;
                }

                includesList.AddRange(GetAllCompositionAndAggregationRelationshipPropertyIncludes(compositionRelationshipsOnly, propType, includePath, depth + 1, maxDepth));
            }

            return includesList;
        }

        private IQueryable<T> CreateSearchQuery<T>(IQueryable<T> query, string values)
        {
            List<Expression> andExpressions = new List<Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var ignore = new List<string>() { "" };

            foreach (var value in values.Split('&'))
            {
                List<Expression> orExpressions = new List<Expression>();

                foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string) && !ignore.Contains(x.Name)))
                {
                    MemberExpression member_expression = Expression.PropertyOrField(parameter, prop.Name);

                    ConstantExpression value_expression = Expression.Constant(value, typeof(string));

                    MethodCallExpression contains_expression = Expression.Call(member_expression, contains_method, value_expression);

                    orExpressions.Add(contains_expression);
                }

                if (orExpressions.Count == 0)
                    return query;

                Expression or_expression = orExpressions[0];

                for (int i = 1; i < orExpressions.Count; i++)
                {
                    or_expression = Expression.OrElse(or_expression, orExpressions[i]);
                }

                andExpressions.Add(or_expression);
            }

            if (andExpressions.Count == 0)
                return query;

            Expression and_expression = andExpressions[0];

            for (int i = 1; i < andExpressions.Count; i++)
            {
                and_expression = Expression.AndAlso(and_expression, andExpressions[i]);
            }

            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(
                and_expression, parameter);

            return query.Where(expression);
        }

        private void DebugSQL(IQueryable<TEntity> query)
        {
            var sql = query.ToString();
        }

        #region SQLQuery
        public virtual IReadOnlyList<TEntity> SQLQuery(string query, params object[] paramaters)
        {
            return context.Set<TEntity>().AsNoTracking().FromSql(query, paramaters).ToList();
        }

        public async virtual Task<IReadOnlyList<TEntity>> SQLQueryAsync(string query, params object[] paramaters)
        {
            return await context.Set<TEntity>().AsNoTracking().FromSql(query, paramaters).ToListAsync();
        }
        #endregion

        #region GetAll
        public virtual IReadOnlyList<TEntity> GetAll(
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         int? skip = null,
         int? take = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, null, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList().AsReadOnly();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, null, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IReadOnlyList<TEntity> GetAllNoTracking(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? skip = null,
        int? take = null,
        bool getAggregate = false,
        bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, null, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllNoTrackingAsync(CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, null, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Get
        public virtual IReadOnlyList<TEntity> Get(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IReadOnlyList<TEntity> GetNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetNoTrackingAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, null, filter).Count();
        }

        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, null, filter).CountAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Search
        public virtual IReadOnlyList<TEntity> Search(
          string ownedBy = null,
          string search = "",
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, ownedBy, search, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList();
        }

        public virtual async Task<IReadOnlyList<TEntity>> SearchAsync(CancellationToken cancellationToken,
             string ownedBy = null,
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
           bool getAggregate = false,
           bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, ownedBy, search, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual IReadOnlyList<TEntity> SearchNoTracking(
          string ownedBy = null,
          string search = "",
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, ownedBy, search, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToList();
        }

        public virtual async Task<IReadOnlyList<TEntity>> SearchNoTrackingAsync(CancellationToken cancellationToken,
            string ownedBy = null,
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool getAggregate = false,
            bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, ownedBy, search, filter, orderBy, skip, take, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetSearchCount(string ownedBy = null, string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, ownedBy, search, filter).Count();
        }

        public virtual async Task<int> GetSearchCountAsync(CancellationToken cancellationToken, string ownedBy = null, string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, ownedBy, search, filter).CountAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetOne
        public virtual TEntity GetOne(
         Expression<Func<TEntity, bool>> filter = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(CancellationToken cancellationToken,
          Expression<Func<TEntity, bool>> filter = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetOneNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneNoTrackingAsync(CancellationToken cancellationToken,
          Expression<Func<TEntity, bool>> filter = null,
          bool getAggregate = false,
          bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetFirst
        public virtual TEntity GetFirst(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           bool getAggregate = false,
           bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, filter, orderBy, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, filter, orderBy, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetFirstNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        bool getAggregate = false,
        bool getAggregateAndAssociatedAggregates = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, filter, orderBy, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstNoTrackingAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, filter, orderBy, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetById
        public virtual TEntity GetById(object id, bool getAggregate = false, bool getAggregateAndAssociatedAggregates = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            if (getAggregate || getAggregateAndAssociatedAggregates || (includeProperties != null && includeProperties.Count() > 0))
            {
                Expression<Func<TEntity, bool>> filter = SearchForEntityById(id);
                return GetQueryable(true, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefault();
            }
            else
            {
                return context.Set<TEntity>().Find(id);
            }
        }

        public virtual TEntity GetByIdNoTracking(object id, bool getAggregate = false, bool getAggregateAndAssociatedAggregates = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = SearchForEntityById(id);
            return GetQueryable(false, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefault();
        }

        public async virtual Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, object id, bool getAggregate = false, bool getAggregateAndAssociatedAggregates = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            if (getAggregate || getAggregateAndAssociatedAggregates || (includeProperties != null && includeProperties.Count() > 0))
            {
                Expression<Func<TEntity, bool>> filter = SearchForEntityById(id);
                return await GetQueryable(true, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await context.Set<TEntity>().FindAsync(id);
            }
        }

        public async virtual Task<TEntity> GetByIdNoTrackingAsync(CancellationToken cancellationToken, object id, bool getAggregate = false, bool getAggregateAndAssociatedAggregates = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = SearchForEntityById(id);
            return await GetQueryable(false, null, null, filter, null, null, null, getAggregate, getAggregateAndAssociatedAggregates, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityById(object id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");
            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var value = Expression.Constant((dynamic)Convert.ChangeType(id, propType));

            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TEntity GetByIdWithPagedCollectionProperty(object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null,
            int? take = null)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                context.LoadCollectionProperty(entity, collectionExpression, search, orderBy, ascending, skip, take);
            }
            return entity;
        }

        public async virtual Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null,
            int? take = null)
        {
            var entity = await GetByIdAsync(cancellationToken, id);
            if (entity != null)
            {
                await context.LoadCollectionPropertyAsync(entity, collectionExpression, search, orderBy, ascending, skip, take, cancellationToken);
            }
            return entity;
        }

        public virtual int GetByIdWithPagedCollectionPropertyCount(object id, string collectionExpression, string search = "")
        {
            var entity = GetById(id);
            if (entity != null)
            {
                return context.CollectionPropertyCount(entity, collectionExpression, search);
            }
            return 0;
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionExpression, string search = "")
        {
            var entity = await GetByIdAsync(cancellationToken, id);
            if (entity != null)
            {
                return await context.CollectionPropertyCountAsync(entity, collectionExpression, search, cancellationToken);
            }
            return 0;
        }
        #endregion

        #region GetByIds
        public virtual IReadOnlyList<TEntity> GetByIds(IEnumerable<object> ids,
      bool getAggregate = false,
      bool getAggregateAndAssociatedAggregates = false,
      params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds(list);
            return GetQueryable(true, null, null, filter, null, null).ToList();
        }

        public virtual IReadOnlyList<TEntity> GetByIdsNoTracking(IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds(list);
            return GetQueryable(false, null, null, filter, null, null).ToList();
        }

        public async virtual Task<IReadOnlyList<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds(list);
            return await GetQueryable(false, null, null, filter, null, null).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<IReadOnlyList<TEntity>> GetByIdsNoTrackingAsync(CancellationToken cancellationToken, IEnumerable<object> ids,
         bool getAggregate = false,
         bool getAggregateAndAssociatedAggregates = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds(list);
            return await GetQueryable(false, null, null, filter, null, null).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityByIds(IEnumerable<object> ids)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");

            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var genericType = typeof(List<>).MakeGenericType(propType);
            var idList = Activator.CreateInstance(genericType);

            var add_method = idList.GetType().GetMethod("Add");
            foreach (var id in ids)
            {
                add_method.Invoke(idList, new object[] { (dynamic)Convert.ChangeType(id, propType) });
            }

            var contains_method = idList.GetType().GetMethod("Contains");
            var value_expression = Expression.Constant(idList);
            var contains_expression = Expression.Call(value_expression, contains_method, prop);
            var lamda = Expression.Lambda<Func<TEntity, bool>>(contains_expression, item);
            return lamda;
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(true, null, null, filter).ToList().Any();
        }

        public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return (await GetQueryable(true, null, null, filter).ToListAsync(cancellationToken).ConfigureAwait(false)).Any();
        }

        public virtual bool ExistsNoTracking(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, null, filter).Any();
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, null, filter).AnyAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool Exists(object id)
        {
            return GetById(id) != null;
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken, object id)
        {
            return (await GetByIdAsync(cancellationToken, id)) != null;
        }

        public bool ExistsNoTracking(object id)
        {
            return GetByIdNoTracking(id) != null;
        }

        public async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, object id)
        {
            return (await GetByIdNoTrackingAsync(cancellationToken, id)) != null;
        }

        public virtual bool Exists(TEntity entity)
        {
            return context.EntityExists(entity);
        }

        public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken, TEntity entity)
        {
            return await context.EntityExistsAsync(entity, cancellationToken);
        }

        public virtual bool ExistsNoTracking(TEntity entity)
        {
            return context.EntityExistsNoTracking(entity);
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, TEntity entity)
        {
            return await context.EntityExistsNoTrackingAsync(entity, cancellationToken);
        }

        public virtual bool ExistsById(object id)
        {
            return context.EntityExistsById<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdAsync(CancellationToken cancellationToken, object id)
        {
            return await context.EntityExistsByIdAsync<TEntity>(id, cancellationToken).ConfigureAwait(false);
        }

        public virtual bool ExistsByIdNoTracking(object id)
        {
            return context.EntityExistsByIdNoTracking<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdNoTrackingAsync(CancellationToken cancellationToken, object id)
        {
            return await context.EntityExistsByIdNoTrackingAsync<TEntity>(id, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region OrderBy
        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy(string orderColumn, string orderType)
        {
            if (string.IsNullOrEmpty(orderColumn))
                return null;

            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            string[] props = orderColumn.Split('.');
            IQueryable<TEntity> query = new List<TEntity>().AsQueryable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            int i = 0;
            foreach (string prop in props)
            {
                var targetProperty = prop;

                PropertyInfo pi = type.GetProperty(targetProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
                i++;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);

            string methodName = (orderType == "asc" || orderType == "OrderBy") ? "OrderBy" : "OrderByDescending";

            var genericTypes = new Type[] { typeof(TEntity), type };

            MethodCallExpression resultExp =
                Expression.Call(typeof(Queryable), methodName, genericTypes, outerExpression.Body, Expression.Quote(lambda));

            var finalLambda = Expression.Lambda(resultExp, argQueryable);

            return ((Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>)finalLambda).Compile();
        }
        #endregion
    }
}
