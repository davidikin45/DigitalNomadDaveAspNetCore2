using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.Categories.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Categories.Services
{
    public interface ICategoryApplicationService : IApplicationServiceEntity<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto>
    {
        Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
