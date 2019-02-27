using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.Domain.Blog.Tags;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.Blog.Tags.Dtos
{
    public class TagDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        public TagDto()
        {
        }

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

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<TagDto, Tag>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<Tag, TagDto>();
        }
    }
}
