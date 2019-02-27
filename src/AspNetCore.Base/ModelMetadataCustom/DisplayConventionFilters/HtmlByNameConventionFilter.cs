using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayConventionFilters
{
    public class HtmlByNameConventionFilter : IDisplayMetadataConventionFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public HtmlByNameConventionFilter(DisplayConventionsDisableSettings displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings;
        }

        private static readonly HashSet<string> TextAreaFieldNames =
                new HashSet<string>
                        {
                            "html"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if(!_displayConventionsDisableSettings.HtmlByName)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;

                if (!string.IsNullOrEmpty(propertyName) &&
                      string.IsNullOrEmpty(modelMetadata.DataTypeName) &&
                      TextAreaFieldNames.Any(propertyName.ToLower().Contains))
                {
                    modelMetadata.DataTypeName = "Html";
                }
            }
        }
    }
}