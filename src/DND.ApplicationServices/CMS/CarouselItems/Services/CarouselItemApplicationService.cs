using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.CarouselItems.Dtos;
using DND.Core;
using DND.Domain.CMS.CarouselItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.CarouselItems.Services
{
    [ResourceCollection(ResourceCollections.CMS.CarouselItems.CollectionId)]
    public class CarouselItemApplicationService : ApplicationServiceEntityBase<CarouselItem, CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, IAppUnitOfWork>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<CarouselItemDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

    }
}
