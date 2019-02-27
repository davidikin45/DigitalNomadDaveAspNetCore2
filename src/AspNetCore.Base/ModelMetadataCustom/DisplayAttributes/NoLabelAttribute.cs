using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    public class NoLabelAttribute : Attribute, IDisplayMetadataAttribute
    {
        public bool NoLabel { get; set; } = true;

        public NoLabelAttribute()
        {
        }

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["NoLabel"] = NoLabel;
        }
    }
}