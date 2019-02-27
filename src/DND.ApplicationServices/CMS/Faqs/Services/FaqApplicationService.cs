using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.Faqs.Dtos;
using DND.Core;
using DND.Domain.CMS.Faqs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    [ResourceCollection(ResourceCollections.CMS.Faqs.CollectionId)]
    public class FaqApplicationService : ApplicationServiceEntityBase<Faq, FaqDto, FaqDto, FaqDto, FaqDeleteDto, IAppUnitOfWork>, IFaqApplicationService
    {
        public FaqApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<FaqDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }
    }
}
