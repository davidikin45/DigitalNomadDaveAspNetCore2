using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    public class HelpTextAttribute : Attribute, IDisplayMetadataAttribute
    {
        public string HelpText { get; set; }

        public HelpTextAttribute(string helpText)
        {
            HelpText = helpText;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["HelpText"] = HelpText;
        }
    }
}