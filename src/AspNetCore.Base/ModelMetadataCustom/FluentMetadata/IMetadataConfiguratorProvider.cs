using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AspNetCore.Base.ModelMetadataCustom.FluentMetadata
{
    public interface IMetadataConfiguratorProviderSingleton
    {
        IEnumerable<IMetadataConfigurator> GetMetadataConfigurators(ModelMetadataIdentity identity);
    }
}