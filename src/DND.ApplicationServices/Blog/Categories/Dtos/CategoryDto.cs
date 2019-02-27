using AspNetCore.Base.Dtos;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.Domain.Blog.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DND.ApplicationServices.Blog.Categories.Dtos
{
    public class CategoryDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        [Render(ShowForCreate = false, ShowForEdit = false, ShowForGrid = false, ShowForDisplay = false)]
        public int Count { get; set; }

        [Required]
        public bool Published { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CategoryDto, Category>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<Category, CategoryDto>();
        }
    }
}
