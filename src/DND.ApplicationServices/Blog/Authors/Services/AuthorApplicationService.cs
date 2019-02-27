using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.Authors.Dtos;
using DND.Core;
using DND.Domain.Blog.Authors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Authors.Services
{
    [ResourceCollection(ResourceCollections.Blog.Authors.CollectionId)]
    public class AuthorApplicationService : ApplicationServiceEntityBase<Author, AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAppUnitOfWork>, IAuthorApplicationService
    {
        public AuthorApplicationService(IAppUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<AuthorDto>> hubContext)
        : base(unitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

        public async Task<AuthorDto> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            var bo = await UnitOfWork.AuthorRepository.GetAuthorAsync(authorSlug, cancellationToken);
            return Mapper.Map<AuthorDto>(bo);
        }

    }
}
