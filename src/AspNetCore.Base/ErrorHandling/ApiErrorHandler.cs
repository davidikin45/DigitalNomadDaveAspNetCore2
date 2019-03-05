using AspnetCore.Base.Validation.Errors;
using AspNetCore.Base.Alerts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace AspNetCore.Base.ErrorHandling
{
    public static class ApiErrorHandler
    {
        public static IActionResult HandleApiException(HttpContext httpContext, ClaimsPrincipal user, Exception exception)
        {

            if (exception is UnauthorizedErrors)
            {
                var errors = (UnauthorizedErrors)exception;

                var errorList = new List<string>();
                foreach (var validationError in errors.Errors)
                {
                    errorList.Add(validationError.PropertyExceptionMessage);
                }
 
                if(user.Identity.IsAuthenticated)
                {
                    return new StatusCodeResult(StatusCodes.Status403Forbidden);
                }
                else
                {
                    return new UnauthorizedResult();
                }
            }
            else if (exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var problemDetails = new ProblemDetails()
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request.",
                    Detail = Messages.RequestCancelled,
                    Instance = httpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest
                };

                var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
                problemDetails.Extensions["traceId"] = traceId;
                problemDetails.Extensions["timeGenerated"] = DateTime.UtcNow;

                var result = new BadRequestObjectResult(problemDetails);
                result.ContentTypes.Add("application/problem+json");
                result.ContentTypes.Add("application/problem+xml");

                return result;
            }
            else if (exception is TimeoutException)
            {
                var result = new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
                return result;
            }

            return null;
        }

        public static (string message, int statusCode) HandleApiExceptionGlobal(HttpContext httpContext, Exception exception, bool showExceptionMessage)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "about:blank",
                Title = Messages.UnknownError,
                Instance = httpContext.Request.Path,
                Status = StatusCodes.Status500InternalServerError
            };

            if (showExceptionMessage && exception != null)
            {
                problemDetails.Detail = exception.Message;
            }

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions["traceId"] = traceId;
            problemDetails.Extensions["timeGenerated"] = DateTime.UtcNow;

            var message = JsonConvert.SerializeObject(problemDetails);

            return (message, StatusCodes.Status500InternalServerError);
        }
    }
}
