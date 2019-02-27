using AspNetCore.Base.Domain;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPostTag : EntityBase<int>
    {
        //[Required]
        public int BlogPostId { get; set; }
        //public virtual BlogPost BlogPost { get; set; }

        //[Required]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public BlogPostTag()
        {

        }
    }
}
