using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    public class BooleanYesNoButtonsAttribute : Attribute, IDisplayMetadataAttribute
    {
        public BooleanYesNoButtonsAttribute()
        {

        }

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "BooleanYesNoButtons";
        }
    }
}