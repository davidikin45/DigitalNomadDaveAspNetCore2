using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.CarouselItems;

namespace DND.Data.Repositories.CMS
{
    public class CarouselItemRepository : GenericRepository<CarouselItem>, ICarouselItemRepository
    {
        public CarouselItemRepository(AppContext context)
            : base(context)
        {

        }
    }
}
