using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Base.Users
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
