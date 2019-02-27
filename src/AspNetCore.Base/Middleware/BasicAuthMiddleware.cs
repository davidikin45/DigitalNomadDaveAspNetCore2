﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace AspNetCore.Base.Middleware
{
    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(
           this IApplicationBuilder builder, string username, string password)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>(username, password);
        }
    }

    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string realm;
        public BasicAuthMiddleware(RequestDelegate next, string realm)
        {
            this.next = next;
            this.realm = realm;
        }
        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ')[1]?.Trim();
                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                // Split username and password
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                // Check if login is correct
                if (IsAuthorized(username, password))
                {
                    await next.Invoke(context);
                    return;
                }
            }
            // Return authentication type (causes browser to show login dialog)
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            // Add realm if it is not null
            if (!string.IsNullOrWhiteSpace(realm))
            {
                context.Response.Headers["WWW-Authenticate"] += $" realm=\"{realm}\"";
            }
            // Return unauthorized
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        // Make your own implementation of this
        public bool IsAuthorized(string username, string password)
        {
            // Check that username and password are correct
            return username.Equals("User1", StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals("SecretPassword!");
        }
    }
}