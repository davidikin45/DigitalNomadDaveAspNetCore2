using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.Blog.Tags.Dtos;
using DND.Domain.Blog.Tags;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostTagDto : DtoBase<int>, IMapFrom<BlogPostTag>, IMapTo<BlogPostTag>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required]
        [Dropdown(typeof(Tag), nameof(DND.Domain.Blog.Tags.Tag.Name))]
        public int TagId { get; set; }

        [HiddenInput()]
        public int BlogPostId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public TagDto Tag { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }
    }
}
