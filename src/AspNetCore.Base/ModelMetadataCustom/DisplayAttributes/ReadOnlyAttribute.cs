﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.DisplayAttributes
{
    public class ReadOnlyAttribute : Attribute, IDisplayMetadataAttribute
    {
        public void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            if (string.IsNullOrEmpty(modelMetadata.DataTypeName))
            {
                modelMetadata.DataTypeName = "ReadOnly";
            }
        }
    }
}