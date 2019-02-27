using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.Blog.Categories;

namespace DND.ApplicationServices.Blog.Categories.Dtos
{
    public class CategoryDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Category>, IMapTo<Category>
    {

    }
}
