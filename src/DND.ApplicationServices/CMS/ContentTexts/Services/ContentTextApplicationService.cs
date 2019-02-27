using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.ContentTexts.Dtos;
using DND.Core;
using DND.Domain.CMS.ContentTexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.ContentTexts.Services
{
    [ResourceCollection(ResourceCollections.CMS.ContentTexts.CollectionId)]
    public class ContentTextApplicationService : ApplicationServiceEntityBase<ContentText, ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IAppUnitOfWork>, IContentTextApplicationService
    {
        public ContentTextApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<ContentTextDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }
    }
}
