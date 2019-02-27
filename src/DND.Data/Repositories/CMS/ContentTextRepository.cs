using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.ContentTexts;

namespace DND.Data.Repositories.CMS
{
    public class ContentTextRepository : GenericRepository<ContentText>, IContentTextRepository
    {
        public ContentTextRepository(AppContext context)
            : base(context)
        {

        }
    }
}
