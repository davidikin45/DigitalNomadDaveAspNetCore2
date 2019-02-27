using AspNetCore.Base.Domain;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Categories;
using System;
using System.Collections.Generic;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPost : EntityAggregateRootBase<int>
    {

        //[Required, StringLength(500)]
        public string Title
        { get; set; }

        //[Required, StringLength(5000)]
        public string ShortDescription
        { get; set; }

        //[Required, StringLength(30000)]
        public string Description
        { get; set; }

        //[Required, StringLength(200)]
        public string UrlSlug
        { get; set; }

        public string CarouselImage
        { get; set; }

        //[StringLength(200)]
        public string CarouselText
        { get; set; }

        //[Required]
        public bool ShowInCarousel
        { get; set; }

        //[Required]
        public bool Published
        { get; set; }

        //[Required]
        public override DateTime CreatedOn
        {
            get { return base.CreatedOn; }
            set { base.CreatedOn = value; }
        }

        //[Required]
        public int AuthorId { get; set; }
        public virtual Author Author
        { get; set; }

        //[Required]
        public int CategoryId { get; set; }
        public virtual Category Category
        { get; set; }

        public virtual IList<BlogPostTag> Tags
        { get; set; }

        public virtual IList<BlogPostLocation> Locations
        { get; set; }

        //[Required]
        public bool ShowLocationDetail { get; set; }

        //[Required]
        public bool ShowLocationMap { get; set; }

        //[Required]
        public int MapHeight { get; set; }

        //[Required]
        public int MapZoom { get; set; }

        //[Required]
        public string Album { get; set; }

        //[Required]
        public bool ShowPhotosInAlbum { get; set; }

        public BlogPost()
        {
            Tags = new List<BlogPostTag>();
            Locations = new List<BlogPostLocation>();
        }

    }
}
