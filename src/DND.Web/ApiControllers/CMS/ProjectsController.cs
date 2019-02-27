using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.Projects.Dtos;
using DND.ApplicationServices.CMS.Projects.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers.CMS
{
    [ResourceCollection(ResourceCollections.CMS.Projects.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/cms/projects")]
    public class ProjectsController : ApiControllerEntityAuthorizeBase<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectApplicationService>
    {
        public ProjectsController(IProjectApplicationService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
