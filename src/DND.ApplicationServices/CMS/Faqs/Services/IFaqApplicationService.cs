using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.Faqs.Dtos;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    public interface IFaqApplicationService : IApplicationServiceEntity<FaqDto, FaqDto, FaqDto, FaqDeleteDto>
    {
    }
}
