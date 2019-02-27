using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.CMS.Projects.Dtos;
using DND.ApplicationServices.CMS.Projects.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Projects
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.CMS.Projects.CollectionId)]
    [Route("admin/cms/projects")]
    public class ProjectsController : MvcControllerEntityAuthorizeBase<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectApplicationService>
    {
        public ProjectsController(IProjectApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
