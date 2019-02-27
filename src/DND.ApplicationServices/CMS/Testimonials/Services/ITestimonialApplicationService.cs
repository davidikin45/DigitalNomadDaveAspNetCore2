using AspNetCore.Base.ApplicationServices;
using DND.ApplicationServices.CMS.Testimonials.Dtos;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    public interface ITestimonialApplicationService : IApplicationServiceEntity<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto>
    {
        
    }
}
