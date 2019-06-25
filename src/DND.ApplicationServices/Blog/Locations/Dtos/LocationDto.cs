using AspNetCore.Base.Attributes.Display;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.MvcExtensions;
using AspNetCore.Mvc.Extensions.Attributes.Display;
using AutoMapper;
using DND.Domain.Blog.Locations;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;

namespace DND.ApplicationServices.Blog.Locations.Dtos
{
    public class LocationDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required]
        public string Name { get; set; }
        public LocationType LocationType { get; set; }
        [Render(ShowForGrid = false)]
        public string DescriptionBody { get; set; }
        public string TimeRequired { get; set; }
        public string Cost { get; set; }
        [UIHint("String")]
        public string LinkText { get; set; }
        public string Link { get; set; }
        public Boolean ShowOnTravelMap { get; set; }
        public Boolean CurrentLocation { get; set; }
        public Boolean NextLocation { get; set; }

        [Render(AllowSortForGrid = false)]
        //[Required]
        [FolderAppSettingsDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        //[Dropdown(typeof(User), nameof(User.Name))]
        //public string UserId { get; set; }
        public string PlaceId { get; set; }
        public Point GPSLocation { get; set; }

        [StringLength(200, ErrorMessage = "UrlSlug: length should not exceed 200 characters")]
        public string UrlSlug { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime CreatedOn { get; set; }

        public LocationDto()
        {
            LocationType = LocationType.City;
            CreatedOn = DateTime.Now;
        }

        public Boolean HasGPSCoordinates()
        {
            return GPSLocation != null && GPSLocation.Y != default(double) && GPSLocation.X != default(double);
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<LocationDto, Domain.Blog.Locations.Location>()
             .ForMember(bo => bo.UpdatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.CreatedOn, dto => dto.Ignore())
            .ForMember(bo => bo.Latitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.Y : (double?)null))
           .ForMember(bo => bo.Longitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.X : (double?)null));

            configuration.CreateMap<Domain.Blog.Locations.Location, LocationDto>()
             .ForMember(dto => dto.GPSLocation, bo => bo.MapFrom(s => s.Latitude.HasValue && s.Longitude.HasValue ?
             GeographyExtensions.CreatePoint(s.Latitude.Value, s.Longitude.Value) : default(Point)));
        }
    }
}
