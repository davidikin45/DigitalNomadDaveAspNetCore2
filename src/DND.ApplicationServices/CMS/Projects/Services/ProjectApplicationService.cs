using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.Projects.Dtos;
using DND.Core;
using DND.Domain.CMS.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Projects.Services
{
    [ResourceCollection(ResourceCollections.CMS.Projects.CollectionId)]
    public class ProjectApplicationService : ApplicationServiceEntityBase<Project, ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IAppUnitOfWork>, IProjectApplicationService
    {
        public ProjectApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<ProjectDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }
    }
}
