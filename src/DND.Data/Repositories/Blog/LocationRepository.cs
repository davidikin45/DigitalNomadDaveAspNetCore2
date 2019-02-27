using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Helpers;
using DND.Core.Repositories.Blog;
using DND.Domain.Blog.Locations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Data.Repositories.Blog
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(AppContext context)
            :base(context)
        {

        }

        public async Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            return await GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug)).ConfigureAwait(false);
        }

        public override Location Add(Location entity, string addedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Add(entity, addedBy);
        }

        public override Location Update(Location entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Update(entity, updatedBy);
        }
    }
}
