using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.ApplicationServices.Blog.Locations.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Locations
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.Blog.Locations.CollectionId)]
    [Route("admin/blog/locations")]
    public class LocationsController : MvcControllerEntityAuthorizeBase<LocationDto, LocationDto, LocationDto, LocationDeleteDto, ILocationApplicationService>
    {
        public LocationsController(ILocationApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }

    }
}
