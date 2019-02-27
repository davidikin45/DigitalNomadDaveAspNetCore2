using AspNetCore.Base.Domain;

namespace DND.Domain.Blog.Categories
{
    public class Category : EntityAggregateRootBase<int>
    {
        //[Required, StringLength(50)]
        public string Name
        { get; set; }

        //[Required, StringLength(50)]
        public string UrlSlug
        { get; set; }

        //[Required, StringLength(200)]
        public string Description
        { get; set; }

        //public virtual IList<Post> Posts
        //{ get; set; }

        public bool Published
        { get; set; }
    }
}
