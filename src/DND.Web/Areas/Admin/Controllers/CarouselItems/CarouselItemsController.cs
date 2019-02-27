using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;
using DND.ApplicationServices.CMS.CarouselItems.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.CarouselItems
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.CarouselItems.CollectionId)]
    [Route("admin/cms/carousel-items")]
    public class CarouselItemsController : MvcControllerEntityAuthorizeBase<CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, ICarouselItemApplicationService>
    {
        public CarouselItemsController(ICarouselItemApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }

    }
}
