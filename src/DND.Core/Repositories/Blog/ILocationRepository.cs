using AspNetCore.Base.Data.Repository;
using DND.Domain.Blog.Locations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Core.Repositories.Blog
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
