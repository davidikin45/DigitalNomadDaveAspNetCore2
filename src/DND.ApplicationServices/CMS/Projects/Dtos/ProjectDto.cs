using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.Core;
using DND.Domain.CMS.Projects;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.CMS.Projects.Dtos
{
    public class ProjectDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Link { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileDropdown(Folders.Projects, true)]
        public string File { get; set; }

        [Render(AllowSortForGrid = false)]
        [FolderDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        [Required, StringLength(200)]
        public string DescriptionText { get; set; }


        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ProjectDto, Project>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<Project, ProjectDto>();
        }
    }
}
