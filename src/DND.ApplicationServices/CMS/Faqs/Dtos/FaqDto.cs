using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.Domain.CMS.Faqs;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.CMS.Faqs.Dtos
{
    public class FaqDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required]
        public string Question { get; set; }

        [Required]
    
        [MultilineText(HTML = true)]
        public string Answer { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FaqDto, Faq>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<Faq, FaqDto>();
        }
    }
}
