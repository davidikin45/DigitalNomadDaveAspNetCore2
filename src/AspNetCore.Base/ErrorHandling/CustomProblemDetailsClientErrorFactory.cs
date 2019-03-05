using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace AspNetCore.Base.ErrorHandling
{
    public static class ProblemDetailsServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomProblemDetailsClientErrorFactory(this IServiceCollection services)
        {
            return services.AddSingleton<IClientErrorFactory, CustomProblemDetailsClientErrorFactory>();
        }
    }

    public class CustomProblemDetailsClientErrorFactory : IClientErrorFactory
    {
        private static readonly string TraceIdentifierKey = "traceId";
        public static readonly string TimeGeneratedKey = "timeGenerated";
        private readonly ApiBehaviorOptions _options;

        public CustomProblemDetailsClientErrorFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = actionContext.HttpContext.Request.Path,
                Title = "",
                Status = clientError.StatusCode,
                Type = "about:blank",
            };

            if (clientError.StatusCode is int statusCode &&
                _options.ClientErrorMapping.TryGetValue(statusCode, out var errorData))
            {
                problemDetails.Title = errorData.Title;
                problemDetails.Type = errorData.Link;

                SetTraceId(actionContext, problemDetails);
                SetTimeGenerated(actionContext, problemDetails);
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status,
                ContentTypes =
                {
                    "application/problem+json",
                    "application/problem+xml",
                },
            };
        }

        internal static void SetTraceId(ActionContext actionContext, ProblemDetails problemDetails)
        {
            var traceId = Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier;
            problemDetails.Extensions[TraceIdentifierKey] = traceId;
        }

        internal static void SetTimeGenerated(ActionContext actionContext, ProblemDetails problemDetails)
        {
            problemDetails.Extensions[TimeGeneratedKey] = DateTime.UtcNow;
        }
    }
}
