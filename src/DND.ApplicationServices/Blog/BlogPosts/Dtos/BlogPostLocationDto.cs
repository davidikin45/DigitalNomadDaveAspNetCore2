using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Mvc.Extensions.Attributes.Display;
using AutoMapper;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Locations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.Blog.BlogPosts.Dtos
{
    public class BlogPostLocationDto : DtoBase<int>, IMapFrom<BlogPostLocation>, IMapTo<BlogPostLocation>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required]
        [Dropdown(typeof(Location), "{" + nameof(DND.Domain.Blog.Locations.Location.LocationTypeDescription) + "} - {" + nameof(DND.Domain.Blog.Locations.Location.Name) + "}")]
        public int LocationId { get; set; }

        [HiddenInput()]
        public int BlogPostId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public LocationDto Location { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }
    }
}
