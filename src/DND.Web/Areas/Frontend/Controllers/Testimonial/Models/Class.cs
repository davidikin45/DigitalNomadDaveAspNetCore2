using DND.ApplicationServices.CMS.Testimonials.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Testimonial.Models
{
    public class TestimonialsViewModel
    {
        public IList<TestimonialDto> Testimonials { get; set; }
    }
}
