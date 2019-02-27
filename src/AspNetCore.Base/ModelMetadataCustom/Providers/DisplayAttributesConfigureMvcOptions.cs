using AspNetCore.Base.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public class DisplayAttributesConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        public DisplayAttributesConfigureMvcOptions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new AttributeMetadataProvider(_serviceProvider));
        }
    }
}