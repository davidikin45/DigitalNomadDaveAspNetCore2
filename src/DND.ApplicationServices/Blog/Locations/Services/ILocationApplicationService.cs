using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.Blog.Locations.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Locations.Services
{
    public interface ILocationApplicationService : IApplicationServiceEntity<LocationDto, LocationDto, LocationDto, LocationDeleteDto>
    {
        Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
