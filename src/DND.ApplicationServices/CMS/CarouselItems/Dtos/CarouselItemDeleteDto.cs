using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.CarouselItems;

namespace DND.ApplicationServices.CMS.CarouselItems.Dtos
{
    public class CarouselItemDeleteDto : DtoAggregateRootBase<int>, IMapFrom<CarouselItem>, IMapTo<CarouselItem>
    {

        public CarouselItemDeleteDto()
        {

        }
    }
}
