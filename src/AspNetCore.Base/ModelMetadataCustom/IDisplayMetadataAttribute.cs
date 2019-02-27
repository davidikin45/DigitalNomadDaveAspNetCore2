using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom
{
    public interface IDisplayMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context, IServiceProvider serviceProvider);
    }
}
