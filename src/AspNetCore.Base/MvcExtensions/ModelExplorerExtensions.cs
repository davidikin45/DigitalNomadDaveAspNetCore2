using AspNetCore.Base.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AspNetCore.Base.MvcExtensions
{
    public static class ModelExplorerExtensions
    {
        public static IEnumerable<ModelExplorer> ModelExplorerPropertiesRuntime(this IHtmlHelper html)
        {
            var propertiesFields = html.ViewData.ModelExplorer.GetFieldValue("_properties");
            if (propertiesFields == null && html.ViewData.Model is ICustomTypeDescriptor)
            {
                ICustomTypeDescriptor model = html.ViewData.Model as ICustomTypeDescriptor;
                var metadata = GetMetadataForRuntimeType(html);
                var properties = metadata.Properties;

                var propertyDescriptors = model.GetProperties();

                var _properties = new ModelExplorer[properties.Count];
                for (var i = 0; i < properties.Count; i++)
                {
                    var propertyMetadata = properties[i];
                    PropertyDescriptor propertyDescriptor = null;
                    for (var j = 0; j < propertyDescriptors.Count; j++)
                    {
                        if (string.Equals(
                            propertyMetadata.PropertyName,
                            propertyDescriptors[j].Name,
                            StringComparison.Ordinal))
                        {
                            propertyDescriptor = propertyDescriptors[j];
                            break;
                        }
                    }

                    _properties[i] = CreateExplorerForProperty(html.MetadataProvider, html.ViewData.ModelExplorer, propertyMetadata, propertyDescriptor);
                }

                html.ViewData.ModelExplorer.SetFieldValue("_properties", _properties);
            }
            return html.ViewData.ModelExplorer.Properties;
        }

        private static ModelMetadata GetMetadataForRuntimeType(IHtmlHelper html)
        {
            // We want to make sure we're looking at the runtime properties of the model, and for
            // that we need the model metadata of the runtime type.
            var metadata = html.ViewData.ModelExplorer.Metadata;
            if (html.ViewData.ModelExplorer.Metadata.ModelType != html.ViewData.ModelExplorer.ModelType)
            {
                metadata = html.MetadataProvider.GetMetadataForType(html.ViewData.ModelExplorer.ModelType);
            }

            return metadata;
        }

        private static ModelExplorer CreateExplorerForProperty(
        IModelMetadataProvider metadataProvider,
        ModelExplorer modelExplorer,
        ModelMetadata propertyMetadata,
        PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor == null)
            {
                return new ModelExplorer(metadataProvider, modelExplorer, propertyMetadata, modelAccessor: null);
            }

            var modelAccessor = new Func<object, object>((c) =>
            {
                return c == null ? null : propertyDescriptor.GetValue(((ICustomTypeDescriptor)c).GetPropertyOwner(propertyDescriptor));
            });

            return new ModelExplorer(metadataProvider, modelExplorer, propertyMetadata, modelAccessor);
        }
    }
}
