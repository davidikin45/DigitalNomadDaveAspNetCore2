using AspNetCore.Base.Data.Repository;
using DND.Domain.Blog.Tags;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Core.Repositories.Blog
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
