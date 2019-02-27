using System.ComponentModel.DataAnnotations;

namespace DND.Web.Areas.Identity.Controllers.Account.Models
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
