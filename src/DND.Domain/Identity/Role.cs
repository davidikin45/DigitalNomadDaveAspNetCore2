using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Identity
{
    public enum Role
    {
        //[EnumMember(Value = "anonymous")]
        [Display(Name = "Anonymous")]
        anonymous,
        [Display(Name = "Authenticated")]
        authenticated,
        [Display(Name = "Read-Only")]
        read_only,
        [Display(Name = "Admin")]
        admin
    }
}
