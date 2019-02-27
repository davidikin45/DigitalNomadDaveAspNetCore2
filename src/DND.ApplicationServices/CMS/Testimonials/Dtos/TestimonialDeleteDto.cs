using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using DND.Domain.CMS.Testimonials;

namespace DND.ApplicationServices.CMS.Testimonials.Dtos
{
    public class TestimonialDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Testimonial>, IMapTo<Testimonial>
    {

    }
}
