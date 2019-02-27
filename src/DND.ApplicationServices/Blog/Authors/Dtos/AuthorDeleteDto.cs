using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.Blog.Authors;

namespace DND.ApplicationServices.Blog.Authors.Dtos
{
    public class AuthorDeleteDto : DtoAggregateRootBase<int> , IMapFrom<Author>, IMapTo<Author>
    {
        public AuthorDeleteDto()
		{

        }
    }
}