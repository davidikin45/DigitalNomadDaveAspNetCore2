using AspNetCore.Base.Domain;

namespace DND.Domain.CMS.Testimonials
{
    public class Testimonial : EntityAggregateRootBase<int>
    {
        //[Required, StringLength(100)]
        public string Source
        { get; set; }

        //[Required, StringLength(5000)]
        public string QuoteText { get; set; }

        public string File { get; set; }
    }
}
