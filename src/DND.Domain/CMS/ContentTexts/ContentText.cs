using AspNetCore.Base.Domain;

namespace DND.Domain.CMS.ContentTexts
{
    public class ContentText : EntityAggregateRootBase<string>
    {
        public string Text { get; set; }

        public bool PreventDelete { get; set; }
    }
}
