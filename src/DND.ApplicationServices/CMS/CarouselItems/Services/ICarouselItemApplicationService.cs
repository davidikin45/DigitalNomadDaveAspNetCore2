using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;

namespace DND.ApplicationServices.CMS.CarouselItems.Services
{
    public interface ICarouselItemApplicationService : IApplicationServiceEntity<CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto>
    {

    }
}
