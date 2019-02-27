using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Helpers;
using DND.Core.Repositories.Blog;
using DND.Domain.Blog.Authors;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Data.Repositories.Blog
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppContext context)
            :base(context)
        {

        }

        public Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            return GetFirstAsync(cancellationToken, c => c.UrlSlug.Equals(authorSlug));
        }

        public override Author Add(Author entity, string addedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Add(entity, addedBy);
        }

        public override Author Update(Author entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Update(entity, updatedBy);
        }
    }
}
