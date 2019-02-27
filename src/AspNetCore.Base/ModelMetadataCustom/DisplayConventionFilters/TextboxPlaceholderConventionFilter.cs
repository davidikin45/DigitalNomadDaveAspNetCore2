using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayConventionFilters
{
    public class TextboxPlaceholderConventionFilter : IDisplayMetadataConventionFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public TextboxPlaceholderConventionFilter(DisplayConventionsDisableSettings displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableSettings.TextboxPlaceholder)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;
                var displayName = "";
                if (modelMetadata.DisplayName != null)
                {
                    displayName = modelMetadata.DisplayName.Invoke();
                }
                var placeholder = "";
                if (modelMetadata.Placeholder != null)
                {
                    placeholder = modelMetadata.Placeholder.Invoke();
                }

                if (!string.IsNullOrEmpty(displayName) &&
                      string.IsNullOrEmpty(placeholder))
                {
                    context.DisplayMetadata.Placeholder = () => displayName + "...";
                }
            }
        }
    }
}