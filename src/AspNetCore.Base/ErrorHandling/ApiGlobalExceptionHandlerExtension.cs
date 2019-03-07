﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace AspNetCore.Base.ErrorHandling
{
    public static class ApiGlobalExceptionHandlerExtension
    {
        public static IApplicationBuilder UseWebApiExceptionHandler(this IApplicationBuilder app, bool showExceptionMessage)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            return app.UseExceptionHandler(HandleApiException(showExceptionMessage, loggerFactory));
        }

        public static Action<IApplicationBuilder> HandleApiException(bool showExceptionMessage, ILoggerFactory loggerFactory)
        {
            return appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        var logger = loggerFactory.CreateLogger("Global exception logger");
                        var response = ApiErrorHandler.HandleApiExceptionGlobal(context, exceptionHandlerFeature.Error, showExceptionMessage);
                        context.Response.StatusCode = response.statusCode;
                        context.Response.ContentType = "application/problem+json";
                        await context.Response.WriteAsync(response.message);
                    }
                    else
                    {
                        var response = ApiErrorHandler.HandleApiExceptionGlobal(context, null, showExceptionMessage);
                        context.Response.StatusCode = response.statusCode;
                        context.Response.ContentType = "application/problem+json";
                        await context.Response.WriteAsync(response.message);
                    }
                });
            };
        }
    }
}