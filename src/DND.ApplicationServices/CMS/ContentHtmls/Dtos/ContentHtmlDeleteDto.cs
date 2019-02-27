using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.ContentHtmls;

namespace DND.ApplicationServices.CMS.ContentHtmls.Dtos
{
    public class ContentHtmlDeleteDto : DtoAggregateRootBase<string>, IMapTo<ContentHtml>, IMapFrom<ContentHtml>
    {


    }
}
