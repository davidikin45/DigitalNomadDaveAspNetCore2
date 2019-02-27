using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.Blog.Locations;

namespace DND.ApplicationServices.Blog.Locations.Dtos
{
    public class LocationDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Location>, IMapTo<Location>
    {
        public LocationDeleteDto()
        {

        }
    }
}
