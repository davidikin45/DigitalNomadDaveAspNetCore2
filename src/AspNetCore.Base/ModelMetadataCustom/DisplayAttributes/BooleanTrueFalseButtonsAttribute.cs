using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    public class BooleanTrueFalseButtonsAttribute : Attribute, IDisplayMetadataAttribute
    {
        public BooleanTrueFalseButtonsAttribute()
        {

        }

        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "BooleanTrueFalseButtons";
        }
    }
}