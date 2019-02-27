using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.Tags.Dtos;
using DND.Core;
using DND.Domain.Blog.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Tags.Services
{
    [ResourceCollection(ResourceCollections.Blog.Tags.CollectionId)]
    public class TagApplicationService : ApplicationServiceEntityBase<Tag, TagDto, TagDto, TagDto, TagDeleteDto, IAppUnitOfWork>, ITagApplicationService
    {
        public TagApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<TagDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

        public async Task<TagDto> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            var bo = await UnitOfWork.TagRepository.GetTagAsync(tagSlug, cancellationToken);
            return Mapper.Map<TagDto>(bo);
        }
    }
}
