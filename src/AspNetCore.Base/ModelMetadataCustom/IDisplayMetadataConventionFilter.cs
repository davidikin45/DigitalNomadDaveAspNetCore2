using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.Base.ModelMetadataCustom
{
    public interface IDisplayMetadataConventionFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
