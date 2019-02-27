using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Mvc
{
    [HtmlTargetElement("tenant")]
    public sealed class TenantTagHelper : TagHelper
    {
        private readonly ITenantService _service;

        public TenantTagHelper(ITenantService service)
        {
            this._service = service;
        }

        [HtmlAttributeName("tenant-id")]
        public string TenantId { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (TenantId != this._service.GetTenantId())
            {
                output.SuppressOutput();
            }

            return base.ProcessAsync(context, output);
        }
    }
}
