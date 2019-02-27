using DND.ApplicationServices.Blog.Authors.Dtos;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.ApplicationServices.Blog.Tags.Dtos;
using System.Collections.Generic;

namespace DND.Web.Areas.Frontend.Controllers.Blog.Models
{
    public class BlogPostListViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<BlogPostDto> Posts { get; set; }
        public int TotalPosts { get; set; }
        public CategoryDto Category { get; set; }
        public TagDto Tag { get; set; }
        public AuthorDto Author { get; set; }
        public string Search { get; set; }
    }
}
