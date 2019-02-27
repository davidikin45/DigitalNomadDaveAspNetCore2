using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.MailingLists.Dtos;
using DND.Core;
using DND.Domain.CMS.MailingLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    [ResourceCollection(ResourceCollections.CMS.MailingList.CollectionId)]
    public class MailingListApplicationService : ApplicationServiceEntityBase<MailingList, MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IAppUnitOfWork>, IMailingListApplicationService
    {
        public MailingListApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<MailingListDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }
    }
}
