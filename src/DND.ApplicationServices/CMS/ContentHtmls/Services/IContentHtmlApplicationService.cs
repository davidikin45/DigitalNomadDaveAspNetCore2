using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.ContentHtmls.Dtos;

namespace DND.ApplicationServices.CMS.ContentHtmls.Services
{
    public interface IContentHtmlApplicationService : IApplicationServiceEntity<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto>
    {
    }
}
