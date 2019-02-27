using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.Faqs;

namespace DND.Data.Repositories.CMS
{
    public class FaqRepository : GenericRepository<Faq>, IFaqRepository
    {
        public FaqRepository(AppContext context)
            : base(context)
        {

        }
    }
}
