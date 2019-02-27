using AspNetCore.Base.ModelMetadataCustom;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public class ConventionsMetadataProvider : IDisplayMetadataProvider, IValidationMetadataProvider
    {
        public ConventionsMetadataProvider() { }

        private readonly IDisplayMetadataConventionFilter[] _metadataFilters;

        public ConventionsMetadataProvider(
            IDisplayMetadataConventionFilter[] metadataFilters)
        {
            _metadataFilters = metadataFilters;
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            Array.ForEach(_metadataFilters, m => m.TransformMetadata(context));
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
           
        }
    }
}
