using AspNetCore.Base.Domain;

namespace DND.Domain.Blog.Authors
{
    public class Author : EntityAggregateRootBase<int>
    {
        //[Required]
        public string Name { get; set; }

        //[StringLength(50)]
        public string UrlSlug { get; set; }

        public Author()
        {

        }
    }
}
