using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public interface IValidationMetadataConfigurator
    {
        void Configure(ValidationMetadata metadata);
    }
}