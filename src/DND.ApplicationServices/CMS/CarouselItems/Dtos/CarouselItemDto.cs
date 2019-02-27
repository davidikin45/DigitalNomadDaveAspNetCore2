using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using DND.Core;
using DND.Domain.CMS.CarouselItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.ApplicationServices.CMS.CarouselItems.Dtos
{
    public class CarouselItemDto : DtoAggregateRootBase<int>, IHaveCustomMappings, IValidatableObject
    {
        [Render(AllowSortForGrid = false)]
        [FolderDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        public string Title { get; set; }

        public string CarouselText
        { get; set; }

        [UIHint("String")]
        public string ButtonText { get; set; }
        public string Link { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileDropdown(Folders.Carousel, true)]
        public string File { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        [Required]
        public bool OpenInNewWindow
        { get; set; }

        [Required]
        public bool Published { get; set; }

        public CarouselItemDto()
        {

        }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();


            if (string.IsNullOrEmpty(Album) && (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(ButtonText) || string.IsNullOrEmpty(Link) || string.IsNullOrEmpty(File)))
            {
                errors.Add(new ValidationResult("If an Album is not selected Title, Button Text, Link and File must be entered"));
            }

            if (!string.IsNullOrEmpty(Album) && !string.IsNullOrEmpty(File))
            {
                errors.Add(new ValidationResult("Please select an Album or File, but not both"));
            }

            return errors;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CarouselItemDto, CarouselItem>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore());

            configuration.CreateMap<CarouselItem, CarouselItemDto>();
        }
    }
}
