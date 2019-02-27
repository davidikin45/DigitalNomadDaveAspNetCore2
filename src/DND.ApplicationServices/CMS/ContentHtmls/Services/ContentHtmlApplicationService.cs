using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.ContentHtmls.Dtos;
using DND.Core;
using DND.Domain.CMS.ContentHtmls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.CMS.ContentHtmls.Services
{
    [ResourceCollection(ResourceCollections.CMS.ContentHtmls.CollectionId)]
    public class ContentHtmlApplicationService : ApplicationServiceEntityBase<ContentHtml, ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IAppUnitOfWork>, IContentHtmlApplicationService
    {
        public ContentHtmlApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<ContentHtmlDto>> hubContext)
       : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

        public override Task<Result> DeleteAsync(ContentHtmlDeleteDto dto, string deletedBy, CancellationToken cancellationToken)
        {
            return base.DeleteAsync(dto, deletedBy, cancellationToken);
        }
    }
}
