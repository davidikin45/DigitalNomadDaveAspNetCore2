using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.Core;
using DND.Domain.Blog.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Categories.Services
{
    [ResourceCollection(ResourceCollections.Blog.Categories.CollectionId)]
    public class CategoryApplicationService : ApplicationServiceEntityBase<Category, CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, IAppUnitOfWork>, ICategoryApplicationService
    {

        public CategoryApplicationService(IAppUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationSerivce, IHubContext<ApiNotificationHub<CategoryDto>> hubContext)
        : base(unitOfWork, mapper, authorizationService, userService, validationSerivce, hubContext)
        {

        }

        public async Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            var bo = await UnitOfWork.CategoryRepository.GetCategoryAsync(categorySlug, cancellationToken);
            return Mapper.Map<CategoryDto>(bo);
        }
    }
}
