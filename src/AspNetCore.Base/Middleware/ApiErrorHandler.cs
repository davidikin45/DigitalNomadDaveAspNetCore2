using AspnetCore.Base.Validation.Errors;
using AspNetCore.Base.Alerts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace AspNetCore.Base.Middleware
{
    public static class ApiErrorHandler
    {
        public static (WebApiMessage message, int statusCode, bool exceptionHandled) HandleApiException(ClaimsPrincipal user, Exception exception)
        {
            bool exceptionHandled = false;
            int statusCode = (int)HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            if (exception is UnauthorizedErrors)
            {
                var errors = (UnauthorizedErrors)exception;

                var errorList = new List<string>();
                foreach (var validationError in errors.Errors)
                {
                    errorList.Add(validationError.PropertyExceptionMessage);
                }

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.Unauthorised, errorList);
                if(user.Identity.IsAuthenticated)
                {
                    statusCode = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    statusCode = (int)HttpStatusCode.Unauthorized;
                }
                exceptionHandled = true;
            }
            else if (exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var errorList = new List<string>();
                errorList.Add(Messages.RequestCancelled);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestCancelled, errorList);
                statusCode = (int)HttpStatusCode.BadRequest;
                exceptionHandled = true;
            }
            else if (exception is TimeoutException)
            {
                var errorList = new List<string>();
                errorList.Add(Messages.RequestTimedOut);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestTimedOut, errorList);
                statusCode = (int)HttpStatusCode.GatewayTimeout;
                exceptionHandled = true;
            }
            else
            {

            }

            return (messageObject, statusCode, exceptionHandled);
        }

        public static (WebApiMessage message, int statusCode, bool exceptionHandled) HandleApiExceptionGlobal(Exception exception, bool showExceptionMessage)
        {
            bool exceptionHandled = true;
            int statusCode = (int)HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            var errorList = new List<string>();

            if(showExceptionMessage)
            {
                errorList.Add(Messages.UnknownError);
            }
            else
            {
                errorList.Add(exception.Message);
            }

            messageObject = WebApiMessage.CreateWebApiMessage(Messages.UnknownError, errorList);
            statusCode = (int)HttpStatusCode.InternalServerError;

            return (messageObject, statusCode, exceptionHandled);
        }
    }
}
