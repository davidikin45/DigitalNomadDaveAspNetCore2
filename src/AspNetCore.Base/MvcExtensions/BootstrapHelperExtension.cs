using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace AspNetCore.Base.MvcExtensions
{
    public static class BootstrapHelperExtension
    {
        public static BootstrapHelper<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> helper)
        {
            return new BootstrapHelper<TModel>(helper);
        }

        public class BootstrapHelper<TModel>
        {

            private readonly IHtmlHelper<TModel> _htmlHelper;


            public BootstrapHelper(IHtmlHelper<TModel> helper)
            {
                _htmlHelper = helper;
            }

            public IHtmlContent BootstrapLabelFor<TProp>(Expression<Func<TModel, TProp>> property)
            {
                var label = _htmlHelper.LabelFor(property, null, new
                {
                    @class = "col-md-2 form-control-label col-form-label"
                });

                return label;
            }
        }

        public static IHtmlContent BootstrapLabel(this IHtmlHelper helper, string propertyName, int cololumns = 2)
        {
            string @class = "col-md-" + cololumns + " form-control-label col-form-label";
            var label = helper.Label(propertyName, null, new
            {
                @class = @class
            });

            return label;
        }

        public static IHtmlContent HelpText(this IHtmlHelper helper, string helpText)
        {
            var small = new TagBuilder("small");
            small.AddCssClass("form-text");
            small.AddCssClass("text-muted");
            small.InnerHtml.SetContent(helpText);

            return small;
        }
    }
}
