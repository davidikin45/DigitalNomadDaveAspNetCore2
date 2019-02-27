using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AspNetCore.Base.ModelMetadataCustom.Providers
{
    public interface ICustomModelMetadataProviderSingleton : IModelMetadataProvider
    {
        IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType, ICustomTypeDescriptor model);
    }
}
