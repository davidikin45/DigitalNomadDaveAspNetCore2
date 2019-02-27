using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.MailingLists.Dtos;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    public interface IMailingListApplicationService : IApplicationServiceEntity<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto>
    {
    }
}
