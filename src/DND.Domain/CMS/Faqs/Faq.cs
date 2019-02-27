using AspNetCore.Base.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Domain.CMS.Faqs
{
    public class Faq : EntityAggregateRootBase<int>
    {
        //[Required]
        public string Question { get; set; }

        //[Required]
        public string Answer { get; set; }

    }
}
