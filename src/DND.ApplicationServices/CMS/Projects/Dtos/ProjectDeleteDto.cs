using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.Projects;

namespace DND.ApplicationServices.CMS.Projects.Dtos
{
    public class ProjectDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Project>, IMapTo<Project>
    {

    }
}
