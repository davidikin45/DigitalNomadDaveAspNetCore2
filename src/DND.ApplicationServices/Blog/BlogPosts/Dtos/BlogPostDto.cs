using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AspNetCore.Base.ModelMetadataCustom.LinkAttributes;
using AutoMapper;
using DND.ApplicationServices.Blog.Authors.Dtos;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.Blog.BlogPosts.Dtos
{
    public class BlogPostDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required(ErrorMessage = "Title: Field is required")]
        [StringLength(500, ErrorMessage = "Title: Length should not exceed 500 characters")]
        public string Title { get; set; }

        [Render(ShowForGrid = false)]
        [Required(ErrorMessage = "Short Description: Field is required")]
        [StringLength(5000, ErrorMessage = " Short Description: length should not exceed 5000 characters")]
        [MultilineText(HTML = false, Rows = 10)]
        public string ShortDescription { get; set; }

        [Render(ShowForGrid = false)]
        [Required(ErrorMessage = "Description: Field is required")]
        [StringLength(30000, ErrorMessage = "Description: length should not exceed 30000 characters")]
        [MultilineText(HTML = true, Rows = 40)]
        public string Description { get; set; }

        [ActionLink("Details", "AdminAuthors")]
        [LinkRouteValue("id", nameof(AuthorId))]
        [Required]
        [Dropdown(typeof(Author), nameof(DND.Domain.Blog.Authors.Author.Name))]
        public int AuthorId { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public AuthorDto Author { get; set; }

        [ActionLink("Details", "AdminCategories")]
        [LinkRouteValue("id", nameof(CategoryId))]
        [Required]
        [Dropdown(typeof(Category), nameof(DND.Domain.Blog.Categories.Category.Name))]
        public int CategoryId { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false)]
        public CategoryDto Category { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [Repeater("{" + nameof(BlogPostTagDto.TagId) + "}")]
        public List<BlogPostTagDto> Tags { get; set; }

        [Render(ShowForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = false, ShowForEdit = true)]
        [Repeater("{" + nameof(BlogPostLocationDto.LocationId) + "}")]
        public List<BlogPostLocationDto> Locations { get; set; }

        [Required]
        public bool ShowLocationDetail { get; set; }

        [Required]
        public bool ShowLocationMap { get; set; }

        [Required]
        public int MapHeight { get; set; }

        [Required]
        public int MapZoom { get; set; }

        [Render(AllowSortForGrid = false)]
        [Required]
        [FolderDropdown(Folders.Gallery)]
        public string Album { get; set; }

        [Required]
        public bool ShowPhotosInAlbum { get; set; }

        [Render(ShowForGrid = false)]
        [FileDropdown(Folders.Gallery, true)]
        public string CarouselImage
        { get; set; }

        [Render(ShowForGrid = false)]
        [StringLength(200)]
        public string CarouselText
        { get; set; }

        [Required]
        public bool ShowInCarousel
        { get; set; }

        [StringLength(200, ErrorMessage = "UrlSlug: length should not exceed 200 characters")]
        public string UrlSlug { get; set; }

        [Required]
        public bool Published { get; set; }

        [Render(ShowForEdit = false)]
        public DateTime? UpdatedOn { get; set; }

        [Render(ShowForEdit = false)]
        public DateTime CreatedOn { get; set; }

        public BlogPostDto()
        {
            MapHeight = 300;
            MapZoom = 7;
            Tags = new List<BlogPostTagDto>();
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<BlogPost, BlogPostDto>();

            configuration.CreateMap<BlogPostDto, BlogPost>()
                .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
                 .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());
        }
    }
}
