using AspNetCore.Mvc.Extensions.Attributes.Display;
using System.ComponentModel.DataAnnotations;

namespace DND.Web.Areas.Frontend.Controllers.Home.Models
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Url]
        public string Website { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [MultilineText]
        public string Message { get; set; }
    }
}
