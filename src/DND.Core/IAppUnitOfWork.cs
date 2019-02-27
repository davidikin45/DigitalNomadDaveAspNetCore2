using AspNetCore.Base.Data.UnitOfWork;
using DND.Core.Repositories.Blog;

namespace DND.Core
{
    public interface IAppUnitOfWork : IUnitOfWork
    {
        IAuthorRepository AuthorRepository { get; }
        IBlogPostRepository BlogPostRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ILocationRepository LocationRepository { get; }
        ITagRepository TagRepository { get; }
    }
}
