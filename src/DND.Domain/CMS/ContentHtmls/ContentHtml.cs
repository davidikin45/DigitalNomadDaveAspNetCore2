using AspNetCore.Base.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentHtmls
{
    public class ContentHtml : EntityAggregateRootBase<string>, IValidatableObject
    {
        public string HTML { get; set; }

        public bool PreventDelete { get; set; }

        public bool Deleted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Deleted && PreventDelete)
            {
               yield return new ValidationResult("This CMS content cannot be deleted");
            }
        }
    }
}
