using AspNetCore.Base.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public class DisplayConventionFiltersConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IDisplayMetadataConventionFilter[] _metadataFilters;

        public DisplayConventionFiltersConfigureMvcOptions(IDisplayMetadataConventionFilter[] metadataFilters)
        {
            _metadataFilters = metadataFilters;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new ConventionsMetadataProvider(_metadataFilters));
        }
    }
}