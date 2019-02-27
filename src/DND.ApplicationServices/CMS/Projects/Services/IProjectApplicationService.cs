using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.Projects.Dtos;

namespace DND.ApplicationServices.CMS.Projects.Services
{
    public interface IProjectApplicationService : IApplicationServiceEntity<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto>
    {
        
    }
}
