using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.Blog.Tags;

namespace DND.ApplicationServices.Blog.Tags.Dtos
{
    public class TagDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Tag>, IMapTo<Tag>
    {

    }
}
