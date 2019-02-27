using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using System.ComponentModel.DataAnnotations;

namespace DND.Web.Areas.Admin.Controllers.MailingList.Models
{
    public class MailingListEmailViewModel
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [MultilineText(HTML = true, Rows = 7)]
        public string Body { get; set; }
    }
}
