using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.Swagger
{
    public class SwaggerAssignSecurityRequirements : IOperationFilter
    {

        public void Apply(Operation operation, OperationFilterContext context)
        {
            IEnumerable<AuthorizeAttribute> authorizeAttributes = new List<AuthorizeAttribute>();

            // Determine if the operation has the Authorize attribute
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor)
            {
                authorizeAttributes = ((ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).MethodInfo.ReflectedType.GetCustomAttributes(typeof(AuthorizeAttribute), true).Select(a => (AuthorizeAttribute)a);
            }

            if (!authorizeAttributes.Any())
                return;

            // Initialize the operation.security property
            if (operation.Security == null)
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();


            // Add the appropriate security definition to the operation
            var securityRequirements = new Dictionary<string, IEnumerable<string>>();

            foreach (var item in authorizeAttributes)
            {
                if (item.AuthenticationSchemes == null || item.AuthenticationSchemes.Contains(JwtBearerDefaults.AuthenticationScheme))
                {
                    if (!securityRequirements.ContainsKey(JwtBearerDefaults.AuthenticationScheme))
                    {
                        securityRequirements.Add(JwtBearerDefaults.AuthenticationScheme, Enumerable.Empty<string>());
                    }
                }
                if (item.AuthenticationSchemes == null || item.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme))
                {
                    if (!securityRequirements.ContainsKey(CookieAuthenticationDefaults.AuthenticationScheme))
                    {
                        securityRequirements.Add(CookieAuthenticationDefaults.AuthenticationScheme, Enumerable.Empty<string>());
                    }
                }
            }

            if (securityRequirements.Count() == 0)
            {
                securityRequirements.Add(CookieAuthenticationDefaults.AuthenticationScheme, Enumerable.Empty<string>());
            }


            operation.Security.Add(securityRequirements);
        }
    }
}
