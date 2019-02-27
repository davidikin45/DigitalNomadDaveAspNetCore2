using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.Blog.BlogPosts;

namespace DND.ApplicationServices.Blog.BlogPosts.Dtos
{
    public class BlogPostDeleteDto : DtoAggregateRootBase<int>,  IMapFrom<BlogPost>, IMapTo<BlogPost>
    {
        public BlogPostDeleteDto()
        {
        }
    }
}
