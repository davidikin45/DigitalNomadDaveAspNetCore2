using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.ContentTexts.Dtos;

namespace DND.ApplicationServices.CMS.ContentTexts.Services
{
    public interface IContentTextApplicationService : IApplicationServiceEntity<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto>
    {
    }
}
