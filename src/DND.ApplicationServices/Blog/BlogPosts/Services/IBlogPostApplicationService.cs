using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.BlogPosts.Services
{
    public interface IBlogPostApplicationService : IApplicationServiceEntity<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto>
    {
        Task<BlogPostDto> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken);

        IEnumerable<BlogPostDto> GetPosts(int pageNo, int pageSize);
        Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPostDto>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDto>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDto>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken);

        //Admin
    }
}
