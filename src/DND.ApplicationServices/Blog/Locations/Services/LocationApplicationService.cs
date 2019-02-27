using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.Core;
using DND.Domain.Blog.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Locations.Services
{
    [ResourceCollection(ResourceCollections.Blog.Locations.CollectionId)]
    public class LocationApplicationService : ApplicationServiceEntityBase<Location, LocationDto, LocationDto, LocationDto, LocationDeleteDto, IAppUnitOfWork>, ILocationApplicationService
    {
        public LocationApplicationService(IAppUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<LocationDto>> hubContext)
        : base(unitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

        public override Task<Result<LocationDto>> CreateAsync(LocationDto dto, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.CreateAsync(dto, createdBy, cancellationToken);
        }

        public async Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            var bo = await UnitOfWork.LocationRepository.GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug));
            return Mapper.Map<LocationDto>(bo);
        }

        public override Task<Result> UpdateAsync(object id, LocationDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.UpdateAsync(id, dto, updatedBy, cancellationToken);
        }
    }
}
