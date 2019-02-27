using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.TagHelpers
{
    [HtmlTargetElement("label", Attributes = DescriptionForAttributeName)]
    public class LabelDescriptionTagHelper : TagHelper
    {
        private const string DescriptionForAttributeName = "asp-description-for";

        public LabelDescriptionTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order
        {
            get
            {
                return -1000;
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeName(DescriptionForAttributeName)]
        public ModelExpression DescriptionFor { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var metadata = DescriptionFor.Metadata;

            if (metadata == null)
            {
                throw new InvalidOperationException(string.Format("No provided metadata ({0})", DescriptionForAttributeName));
            }

            output.Attributes.SetAttribute("id", metadata.PropertyName + "-description");
            output.Attributes.SetAttribute("for", metadata.PropertyName);

            if (!string.IsNullOrWhiteSpace(metadata.Description))
            {
                output.Content.SetContent(metadata.Description);
            }

            return Task.CompletedTask;
        }
    }
}
