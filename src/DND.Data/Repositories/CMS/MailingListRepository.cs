using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.MailingLists;

namespace DND.Data.Repositories.CMS
{
    public class MailingListRepository : GenericRepository<MailingList>, IMailingListRepository
    {
        public MailingListRepository(AppContext context)
            : base(context)
        {

        }
    }
}
