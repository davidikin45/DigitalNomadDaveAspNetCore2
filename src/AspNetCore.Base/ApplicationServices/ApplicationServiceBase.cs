using AspnetCore.Base.Validation.Errors;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Base.ApplicationServices
{
    public abstract class ApplicationServiceBase : IApplicationService
    {
        public IMapper Mapper { get; }

        public IAuthorizationService AuthorizationService { get; }
        public IUserService UserService { get; }
        public IValidationService ValidationService { get; }

        public ApplicationServiceBase(IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService)
        {
            Mapper = mapper;
            AuthorizationService = authorizationService;
            UserService = userService;
            ValidationService = validationService;
        }

        public async Task AuthorizeResourceOperationAsync(params string[] operations)
        {
            string collectionId = null;

            var resourceAttribute = (ResourceCollectionAttribute)this.GetType().GetCustomAttributes(typeof(ResourceCollectionAttribute), true).FirstOrDefault();
            if (resourceAttribute != null)
            {
                collectionId = resourceAttribute.CollectionId;
            }

            bool success = false;
            foreach (var operation in operations)
            {
                var operationName = operation;
                if (!string.IsNullOrWhiteSpace(collectionId))
                {
                    operationName = operationName + "," + collectionId + "." + operationName;
                }

                var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, operationName);
                if (authorizationResult.Succeeded)
                {
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, resourceAttribute.CollectionId + "." + operations.FirstOrDefault())));
            }
        }

        public async Task AuthorizeResourceOperationAsync(object resource, params string[] operations)
        {
            if (resource != null)
            {
                string collectionId = null;

                var resourceAttribute = (ResourceCollectionAttribute)this.GetType().GetCustomAttributes(typeof(ResourceCollectionAttribute), true).FirstOrDefault();
                if (resourceAttribute != null)
                {
                    collectionId = resourceAttribute.CollectionId;
                }

                bool success = true;
                foreach (var operation in operations)
                {
                    var operationName = operation;
                    if (!string.IsNullOrWhiteSpace(collectionId))
                    {
                        operationName = collectionId + "." + operationName;
                    }

                    var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, resource, new OperationAuthorizationRequirement() { Name = operationName });
                    if (!authorizationResult.Succeeded && authorizationResult.Failure.FailCalled)
                    {
                        success = false;
                    }
                    else if (authorizationResult.Succeeded)
                    {
                        success = true;
                    }
                }

                if (!success)
                {
                    throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, resourceAttribute.CollectionId + "." + operations.FirstOrDefault())));
                }
            }
        }

        public Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(Expression<Func<TSource, Object>>[] selectors)
        {
            return AutoMapperHelper.GetMappedIncludes<TSource, TDestination>(Mapper, selectors);
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return AutoMapperHelper.GetMappedSelector<TSource, TDestination, TProperty>(Mapper, selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            return AutoMapperHelper.GetMappedOrderByCompiled<TSource, TDestination>(Mapper, orderBy);
        }
    }
}
