using AspNetCore.Base.Data.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AspNetCore.Base.Mapping
{
    public static class AutoMapperHelper
    {
        public static object SourceDestinationEquivalentExpressionById(Type sourceType, Type destinationType)
        {
            Type funcType = typeof(Func<,,>).MakeGenericType(new[] { sourceType, destinationType, typeof(bool) });

            var itemSource = Expression.Parameter(sourceType, "source");
            var propSource = Expression.PropertyOrField(itemSource, "Id");
            var propTypeSource = sourceType.GetProperty("Id").PropertyType;

            var itemDestination = Expression.Parameter(destinationType, "destination");
            var propDestination = Expression.PropertyOrField(itemDestination, "Id");
            var propTypeDestination = destinationType.GetProperty("Id").PropertyType;

            var equal = Expression.Equal(propSource, propDestination);

            return typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Lambda)).MakeGenericMethod(funcType).Invoke(null, new object[] { equal, new ParameterExpression[] { itemSource, itemDestination } });
        }

        //Expression > Func yes
        //Func > Expression no compiled
        public static Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(IMapper mapper, params Expression<Func<TSource, Object>>[] selectors)
        {
            if (selectors == null)
                return new Expression<Func<TDestination, Object>>[] { };

            List<Expression<Func<TDestination, Object>>> returnList = new List<Expression<Func<TDestination, Object>>>();

            foreach (var selector in selectors)
            {
                returnList.Add(mapper.Map<Expression<Func<TDestination, Object>>>(selector));
            }

            return returnList.ToArray();
        }

        public static Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(IMapper mapper, Expression<Func<TSource, TProperty>> selector)
        {
            return mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);
        }

        public static Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>> GetMappedOrderBy<TSource, TDestination>(IMapper mapper, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            if (orderBy == null)
                return null;

            return mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy);
        }

        public static Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderByCompiled<TSource, TDestination>(IMapper mapper, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            if (orderBy == null)
                return null;

            return mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy).Compile();
        }

        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderByFunc<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            return GetOrderBy<TEntity>(orderColumn, orderType, mapper, sourceTypes).Compile();
        }

        public static Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> GetOrderBy<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
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
                if (sourceTypes != null && mapper != null)
                {
                    targetProperty = mapper.GetDestinationMappedProperty(sourceTypes[i], type, prop).Name;
                }

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

            return (Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>)finalLambda;
        }

        public static Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> GetOrderByIEnumerableFunc<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            return GetOrderByIEnumerable<TEntity>(orderColumn, orderType, mapper, sourceTypes).Compile();
        }

        public static Expression<Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>>> GetOrderByIEnumerable<TEntity>(string orderColumn, string orderType, IMapper mapper = null, List<Type> sourceTypes = null)
        {
            if (string.IsNullOrEmpty(orderColumn))
                return null;

            Type typeQueryable = typeof(IEnumerable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);

            string[] props = orderColumn.Split('.');
            IEnumerable<TEntity> query = new List<TEntity>().AsEnumerable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            int i = 0;
            foreach (string prop in props)
            {
                var targetProperty = prop;
                if (sourceTypes != null && mapper != null)
                {
                    targetProperty = mapper.GetDestinationMappedProperty(sourceTypes[i], type, prop).Name;
                }

                PropertyInfo pi = type.GetProperty(targetProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
                i++;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);

            string methodName = (orderType == "asc" || orderType == "OrderBy") ? "OrderBy" : "OrderByDescending";

            var resultExp =
                Expression.Call(typeof(Enumerable), methodName, new Type[] { typeof(TEntity), lambda.Body.Type }, argQueryable, lambda);

            var finalLambda = Expression.Lambda(resultExp, argQueryable);

            return (Expression<Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>>>)finalLambda;
        }
    }
}
