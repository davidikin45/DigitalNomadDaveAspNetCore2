using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.ApplicationServices.Blog.Locations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.Blog
{
    [ResourceCollection(ResourceCollections.Blog.Locations.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/blog/locations")]
    public class LocationsController : ApiControllerEntityAuthorizeBase<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public LocationsController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
