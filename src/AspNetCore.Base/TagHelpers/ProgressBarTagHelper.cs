using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Base.TagHelpers
{
    [HtmlTargetElement("progressBar")]
    public class ProgressBarTagHelper : TagHelper
    {
        public string Percent { get; set; }
        public string @Class { get; set; }

        public ProgressBarTagHelper()
        {

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(
                $@"<div class=""progress"">
                        <div class=""progress-bar { Class }"" role=""progressbar"" aria-valuenow=""{ Percent }"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: { Percent }%""> { Percent }%</div>
                    </div>");
        }

    }
}