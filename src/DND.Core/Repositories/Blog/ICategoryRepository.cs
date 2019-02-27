using AspNetCore.Base.Data.Repository;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Tags;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Core.Repositories.Blog
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
