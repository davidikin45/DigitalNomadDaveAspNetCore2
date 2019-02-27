using AspNetCore.Base.Data.Repository;
using DND.Core.Repositories.CMS;
using DND.Domain.CMS.Testimonials;

namespace DND.Data.Repositories.CMS
{
    public class TestimonialRepository : GenericRepository<Testimonial>, ITestimonialRepository
    {
        public TestimonialRepository(AppContext context)
            : base(context)
        {

        }
    }
}
