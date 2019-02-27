using AspNetCore.Base.Data.Repository;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.MailingLists;

namespace DND.Core.Repositories.CMS
{
    public interface IMailingListRepository : IGenericRepository<MailingList>
    {
    }
}
