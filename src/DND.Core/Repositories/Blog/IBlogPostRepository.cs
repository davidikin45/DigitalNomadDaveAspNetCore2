using AspNetCore.Base.Data.Repository;
using DND.Domain.Blog.BlogPosts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Core.Repositories.Blog
{
    public interface IBlogPostRepository : IGenericRepository<BlogPost>
    {
        Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken);
        IReadOnlyList<BlogPost> GetPosts(int pageNo, int pageSize);
        Task<IReadOnlyList<BlogPost>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<BlogPost>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken);
        Task<BlogPost> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken);
    }
}
