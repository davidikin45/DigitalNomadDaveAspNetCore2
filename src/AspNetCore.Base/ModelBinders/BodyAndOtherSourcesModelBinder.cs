using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.ModelBinders
{
    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/ModelBinding/ModelBinderFactory.cs
    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/ModelBinding/Binders/BodyModelBinder.cs
    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/Controllers/ControllerBinderDelegateProvider.cs
    //Uses Input Formatters
    public class BodyAndOtherSourcesModelBinder : BodyModelBinder, IModelBinder
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IModelBinderFactory _modelBinderFactory;

        public BodyAndOtherSourcesModelBinder(IHttpRequestStreamReaderFactory readerFactory, ILoggerFactory loggerFactory, IOptions<MvcOptions> options, IModelBinderFactory modelBinderFactory)
            :base(options.Value.InputFormatters, readerFactory, loggerFactory, options.Value)
        {
            _loggerFactory = loggerFactory;
            _modelBinderFactory = modelBinderFactory;
        }

        public new async Task BindModelAsync(ModelBindingContext bindingContext)
        {     
            //Body
            bool tryOtherBindingSources = false;

            await base.BindModelAsync(bindingContext);

            foreach (var kvp in bindingContext.ModelState)
            {
                foreach (var modelError in kvp.Value.Errors)
                {
                    if (modelError.Exception != null && modelError.Exception.GetType() == typeof(UnsupportedContentTypeException))
                    {
                        if (bindingContext.BindingSource == null || !bindingContext.BindingSource.CanAcceptDataFrom(BindingSource.Body))
                        {
                            tryOtherBindingSources = true;
                        }
                    }
                }
            }

            //Route/Form/Query
            if (tryOtherBindingSources)
            {
                bindingContext.ModelState.Clear();

                var binder = GetBinder(bindingContext.ModelMetadata);

                await binder.BindModelAsync(bindingContext);
            }
        }

        private IModelBinder GetBinder(ModelMetadata modelMetadata)
        {
            var factoryContext = new ModelBinderFactoryContext()
            {
                Metadata = modelMetadata,
                BindingInfo = new BindingInfo()
                {
                    BinderModelName = modelMetadata.BinderModelName,
                    BinderType = modelMetadata.BinderType,
                    BindingSource = modelMetadata.BindingSource,
                    PropertyFilterProvider = modelMetadata.PropertyFilterProvider,
                },

                // We're using the model metadata as the cache token here so that TryUpdateModelAsync calls
                // for the same model type can share a binder. This won't overlap with normal model binding
                // operations because they use the ParameterDescriptor for the token.
                CacheToken = modelMetadata,
            };
            var binder = _modelBinderFactory.CreateBinder(factoryContext);
            return binder;
        }
    }
}
