using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayConventionFilters
{
    public class TextAreaByNameConventionFilter : IDisplayMetadataConventionFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public TextAreaByNameConventionFilter(DisplayConventionsDisableSettings displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings;
        }

        private static readonly HashSet<string> TextAreaFieldNames =
				new HashSet<string>
						{
							"body",
                            "comments"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableSettings.TextAreaByName)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;

                if (!string.IsNullOrEmpty(propertyName) &&
                    string.IsNullOrEmpty(modelMetadata.DataTypeName) &&
                    TextAreaFieldNames.Any(propertyName.ToLower().Contains))
                {
                    modelMetadata.DataTypeName = "MultilineText";
                    modelMetadata.AdditionalValues["MultilineTextRows"] = 7;
                    modelMetadata.AdditionalValues["MultilineTextHTML"] = false;
                }
            }
        }
    }
}