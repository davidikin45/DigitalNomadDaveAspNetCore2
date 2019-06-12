using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.Base.Filters
{
    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.ViewFeatures/src/ViewResultExecutor.cs
    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.ViewFeatures/src/ViewExecutor.cs

    //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/Infrastructure/ObjectResultExecutor.cs

    //Works with [FromBodyRouteQueryAttribute]

    public class ConvertViewResultToObjectResultAttribute : TypeFilterAttribute
    {
        public bool Enabled { get; set; } = true;

        public ConvertViewResultToObjectResultAttribute() : base(typeof(ConvertViewResultToObjectResultAttributeImpl))
        {
            Arguments = new object[] { Enabled };
        }

        private class ConvertViewResultToObjectResultAttributeImpl : ActionFilterAttribute
        {
            private readonly ILogger _logger;
            private readonly MvcOptions _mvcOptions;
            private readonly bool _enabled;
            private readonly ApiBehaviorOptions _apiBehaviorOptions;

            public ConvertViewResultToObjectResultAttributeImpl(OutputFormatterSelector formatterSelector, ILoggerFactory loggerFactory, IOptions<MvcOptions> mvcOptions, IOptions<ApiBehaviorOptions> apiBehaviorOptions, bool enabled)
            {
                _logger = loggerFactory.CreateLogger<ConvertViewResultToObjectResultAttribute>();
                _mvcOptions = mvcOptions.Value;
                _apiBehaviorOptions = apiBehaviorOptions.Value;
                _enabled = enabled;
            }

            public override void OnResultExecuting(ResultExecutingContext context)
            {
                if (_enabled && context.Result is ViewResult)
                {
                    var viewResult = context.Result as ViewResult;

                    var responseContentType = context.HttpContext.Response.ContentType;

                    //If no response content type has been set we can convert ViewResult > ObjectResult
                    if (string.IsNullOrEmpty(responseContentType))
                    {
                        var result = new ObjectResult(viewResult.Model);

                        var objectType = result.DeclaredType;
                        if (objectType == null || objectType == typeof(object))
                        {
                            objectType = result.Value?.GetType();
                        }

                        Func<Stream, Encoding, TextWriter> writerFactory = (stream, encoding) => null;
                        var formatterContext = new OutputFormatterWriteContext(
                            context.HttpContext,
                            writerFactory,
                            objectType,
                            result.Value);

                        //NUll is tricky as all output formatters can write null.
                        var sortedAcceptHeaders = GetAcceptableMediaTypes(context.HttpContext.Request);
                        bool textHtmlRequest;
                        var selectedFormatter = SelectFormatterUsingSortedAcceptHeaders(formatterContext, _mvcOptions.OutputFormatters, sortedAcceptHeaders, out textHtmlRequest);

                        if (selectedFormatter == null)
                        {
                            if(!textHtmlRequest)
                            {
                                _logger.LogInformation($"Failed converting ViewResult > ObjectResult. No output formatter found for Accept Header.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"Successfully converted ViewResult > ObjectResult.");

                            if (!context.ModelState.IsValid)
                            {
                                context.Result = _apiBehaviorOptions.InvalidModelStateResponseFactory(context);
                            }
                            else
                            {
                                context.Result = new ObjectResult(viewResult.Model);
                            }
                        }
                    }
                }
            }

            private readonly Comparison<MediaTypeSegmentWithQuality> _sortFunction = (left, right) =>
            {
                return left.Quality > right.Quality ? -1 : (left.Quality == right.Quality ? 0 : 1);
            };

            private List<MediaTypeSegmentWithQuality> GetAcceptableMediaTypes(HttpRequest request)
            {
                var result = new List<MediaTypeSegmentWithQuality>();
                AcceptHeaderParser.ParseAcceptHeader(request.Headers[HeaderNames.Accept], result);
                for (var i = 0; i < result.Count; i++)
                {
                    var mediaType = new MediaType(result[i].MediaType);
                    if (!_mvcOptions.RespectBrowserAcceptHeader && mediaType.MatchesAllSubTypes && mediaType.MatchesAllTypes)
                    {
                        result.Clear();
                        return result;
                    }
                }

                result.Sort(_sortFunction);

                return result;
            }

            private IOutputFormatter SelectFormatterUsingSortedAcceptHeaders(
            OutputFormatterCanWriteContext formatterContext,
            IList<IOutputFormatter> formatters,
            IList<MediaTypeSegmentWithQuality> sortedAcceptHeaders, out bool textHtmlRequest)
            {
                if (formatterContext == null)
                {
                    throw new ArgumentNullException(nameof(formatterContext));
                }

                if (formatters == null)
                {
                    throw new ArgumentNullException(nameof(formatters));
                }

                if (sortedAcceptHeaders == null)
                {
                    throw new ArgumentNullException(nameof(sortedAcceptHeaders));
                }

                for (var i = 0; i < sortedAcceptHeaders.Count; i++)
                {
                    var mediaType = sortedAcceptHeaders[i];

                    formatterContext.ContentType = mediaType.MediaType;
                    formatterContext.ContentTypeIsServerDefined = false;

                    if (mediaType.MediaType == "text/html")
                    {
                        textHtmlRequest = true;
                        return null;
                    }

                    for (var j = 0; j < formatters.Count; j++)
                    {
                        var formatter = formatters[j];

                        if(formatter is OutputFormatter)
                        {
                            var outputForamtter = formatter as OutputFormatter;
                            if (outputForamtter.CanWriteResult(formatterContext) && OutputFormatterSupportsMediaType(outputForamtter, formatterContext))
                            {
                                textHtmlRequest = false;
                                return formatter;
                            }
                        }
                    }
                }

                textHtmlRequest = false;
                return null;
            }

            private bool OutputFormatterSupportsMediaType(OutputFormatter outputForamtter, OutputFormatterCanWriteContext context)
            {
                var parsedContentType = new MediaType(context.ContentType);
                for (var i = 0; i < outputForamtter.SupportedMediaTypes.Count; i++)
                {
                    var supportedMediaType = new MediaType(outputForamtter.SupportedMediaTypes[i]);
                    if (supportedMediaType.HasWildcard)
                    {
                        // For supported media types that are wildcard patterns, confirm that the requested
                        // media type satisfies the wildcard pattern (e.g., if "text/entity+json;v=2" requested
                        // and formatter supports "text/*+json").
                        // We only do this when comparing against server-defined content types (e.g., those
                        // from [Produces] or Response.ContentType), otherwise we'd potentially be reflecting
                        // back arbitrary Accept header values.
                        if (context.ContentTypeIsServerDefined
                            && parsedContentType.IsSubsetOf(supportedMediaType))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // For supported media types that are not wildcard patterns, confirm that this formatter
                        // supports a more specific media type than requested e.g. OK if "text/*" requested and
                        // formatter supports "text/plain".
                        // contentType is typically what we got in an Accept header.
                        if (supportedMediaType.IsSubsetOf(parsedContentType))
                        {
                            context.ContentType = new StringSegment(outputForamtter.SupportedMediaTypes[i]);
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
