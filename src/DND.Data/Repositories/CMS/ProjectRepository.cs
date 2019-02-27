using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.Projects;

namespace DND.Data.Repositories.CMS
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppContext context)
            : base(context)
        {

        }
    }
}
