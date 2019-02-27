using System.ComponentModel.DataAnnotations;

namespace DND.Web.Areas.Identity.Controllers.Account.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
