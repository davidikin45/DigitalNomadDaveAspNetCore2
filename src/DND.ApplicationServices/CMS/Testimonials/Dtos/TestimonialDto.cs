using AspNetCore.Base.Attributes.Display;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Mvc.Extensions.Attributes.Display;
using AutoMapper;
using DND.Domain.CMS.Testimonials;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.CMS.Testimonials.Dtos
{
    public class TestimonialDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required, StringLength(100)]
        public string Source { get; set; }

        [Required, StringLength(5000)]
        public string QuoteText { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileAppSettingsDropdown(Folders.Testimonials, true)]
        public string File { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<TestimonialDto, Testimonial>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<Testimonial, TestimonialDto>();
        }
    }
}
