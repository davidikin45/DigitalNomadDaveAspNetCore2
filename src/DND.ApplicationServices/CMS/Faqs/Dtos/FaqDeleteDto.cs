using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.Faqs;

namespace DND.ApplicationServices.CMS.Faqs.Dtos
{
    public class FaqDeleteDto : DtoAggregateRootBase<int>, IMapTo<Faq>, IMapFrom<Faq>
    {

    }
}
