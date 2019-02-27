using AspNetCore.Base.ModelMetadataCustom;
using AspNetCore.Base.ModelMetadataCustom.FluentMetadata;
using AspNetCore.Base.ModelMetadataCustom.Providers;
using GeoAPI.Geometries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using System;

namespace AspNetCore.Base
{
    //https://andrewlock.net/accessing-services-when-configuring-mvcoptions-in-asp-net-core/
    public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        public ConfigureMvcOptions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Point)));
        }
    }
}
