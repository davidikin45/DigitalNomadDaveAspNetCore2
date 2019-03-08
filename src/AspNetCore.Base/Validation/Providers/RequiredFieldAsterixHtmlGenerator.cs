using AspNetCore.Base.Extensions;
using AspNetCore.Base.MvcExtensions;
using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;

namespace AspNetCore.Base.Validation.Providers
{
    public static class RequiredFieldAsterixHtmlGeneratorServiceCollectionExtensions
    {
        public static IServiceCollection AddRequiredFieldAsterixHtmlGenerator(this IServiceCollection services)
        {
            services.RemoveAll<IHtmlGenerator>();
            return services.AddSingleton<IHtmlGenerator, RequiredFieldAsterixHtmlGenerator>();
        }
    }

    public class RequiredFieldAsterixHtmlGenerator : DefaultHtmlGenerator, IHtmlGenerator
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;

        public RequiredFieldAsterixHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider, IOptions<DisplayConventionsDisableSettings> displayConventionsDisableSettings)
            : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings.Value;
        }

        public override TagBuilder GenerateLabel(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string labelText, object htmlAttributes)
        {
            var builder = base.GenerateLabel(viewContext, modelExplorer, expression, labelText, htmlAttributes);

            var editOrCreateMode = ((viewContext.ViewData.ContainsKey("EditMode") && (Boolean)viewContext.ViewData["EditMode"]) || (viewContext.ViewData.ContainsKey("CreateMode") && (Boolean)viewContext.ViewData["CreateMode"])) && !(viewContext.ViewData.ContainsKey("DetailsMode") && (Boolean)viewContext.ViewData["DetailsMode"]);

            if (editOrCreateMode && !_displayConventionsDisableSettings.LabelRequiredAsterix && modelExplorer.Metadata.IsRequired)
            {
                var newLabelText = builder.InnerHtml.Render() + " *";
                builder.InnerHtml.SetContent(newLabelText);
            }

            return builder;
        }
    }
}
