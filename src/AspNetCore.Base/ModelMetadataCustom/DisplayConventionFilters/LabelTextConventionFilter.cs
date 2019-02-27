using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayConventionFilters
{
    public class LabelTextConventionFilter : IDisplayMetadataConventionFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public LabelTextConventionFilter(DisplayConventionsDisableSettings displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableSettings.LabelText)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;


                if (IsTransformRequired(propertyName, modelMetadata, propertyAttributes))
                {

                    modelMetadata.DisplayName = () => GetStringWithSpaces(propertyName);
                }
            }
        }

        private bool IsTransformRequired(string propertyName, DisplayMetadata modelMetadata, IReadOnlyList<object> propertyAttributes)
        {
            if (!string.IsNullOrEmpty(modelMetadata.SimpleDisplayProperty))
                return false;

            if (propertyAttributes.OfType<DisplayNameAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<DisplayAttribute>().Any())
                return false;

            if (string.IsNullOrEmpty(propertyName))
                return false;

            return true;
        }

        private string GetStringWithSpaces(string input)
        {
            return Regex.Replace(
               input,
               "(?<!^)" +
               "(" +
               "  [A-Z][a-z] |" +
               "  (?<=[a-z])[A-Z] |" +
               "  (?<![A-Z])[A-Z]$" +
               ")",
               " $1",
               RegexOptions.IgnorePatternWhitespace);
        }
    }
}