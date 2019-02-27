using System.ComponentModel.DataAnnotations;

namespace DND.Web.Areas.Identity.Controllers.Account.Models
{
    public class LoginWithRecoveryCodeViewModel
    {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
    }
}
