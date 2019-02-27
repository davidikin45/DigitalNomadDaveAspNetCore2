using AspNetCore.Base.Domain;
using DND.Domain.Blog.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPostLocation : EntityBase<int>
    {
        //[Required]
        public int BlogPostId { get; set; }
        //public virtual BlogPost BlogPost { get; set; }

        //[Required]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public BlogPostLocation()
        {

        }
    }
}
