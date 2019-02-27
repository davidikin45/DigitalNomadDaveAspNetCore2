using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public interface IDisplayMetadataConfigurator
    {
        void Configure(DisplayMetadata metadata);
    }
}