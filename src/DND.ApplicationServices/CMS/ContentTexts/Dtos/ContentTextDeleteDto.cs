using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.ContentTexts;

namespace DND.ApplicationServices.CMS.ContentTexts.Dtos
{
    public class ContentTextDeleteDto : DtoAggregateRootBase<string>, IMapTo<ContentText>, IMapFrom<ContentText>
    {

    }
}
