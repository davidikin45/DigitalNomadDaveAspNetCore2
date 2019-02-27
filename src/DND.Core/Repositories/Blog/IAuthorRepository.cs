using AspNetCore.Base.Data.Repository;
using DND.Domain.Blog.Authors;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Core.Repositories.Blog
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken);
    }
}
