using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Locations
{
    public enum LocationType
    {
        [Display(Name = "Cafe")]
        Cafe,
        [Display(Name = "Restaurant")]
        Restaurant,
        [Display(Name = "Tourist Attraction")]
        TouristAttraction,
        [Display(Name = "Beach")]
        Beach,
        [Display(Name = "Bar")]
        Bar,
        [Display(Name = "Transport")]
        Transport,
        [Display(Name = "Accomodation")]
        Accomodation,
        [Display(Name = "Airport")]
        Airport,
        [Display(Name = "Region")]
        Region,
        [Display(Name = "City")]
        City,
        [Display(Name = "Country")]
        Country,
        [Display(Name = "Other")]
        Other
    }
}
