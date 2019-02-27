using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ViewComponents;
using DND.ApplicationServices.CMS.Projects.Dtos;
using DND.ApplicationServices.CMS.Projects.Services;
using DND.Web.Areas.Frontend.Controllers.Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Project.ViewComponents
{
    public class ProjectViewComponent : ViewComponentBase
    {
        private readonly IProjectApplicationService _projectService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemRepository;

        public ProjectViewComponent(IProjectApplicationService projectService, IFileSystemGenericRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(ProjectDto.CreatedOn);
            string orderType = "desc";

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<ProjectDto> projects = null;


            var projectsTask = _projectService.GetAllAsync(cts.Token, AutoMapperHelper.GetOrderBy<ProjectDto>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, projectsTask);

            projects = projectsTask.Result;


            var viewModel = new ProjectsViewModel
            {
                Projects = projects.ToList()
            };

            return View(viewModel);
        }

    }
}
