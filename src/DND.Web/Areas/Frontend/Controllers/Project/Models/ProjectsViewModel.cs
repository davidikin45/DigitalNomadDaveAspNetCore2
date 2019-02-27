using DND.ApplicationServices.CMS.Projects.Dtos;
using System.Collections.Generic;

namespace DND.Web.Areas.Frontend.Controllers.Project.Models
{
    public class ProjectsViewModel
    {
        public IList<ProjectDto> Projects { get; set; }
    }
}
