using AspNetCore.Base.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace AspNetCore.Base.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string apiName, string description, string contactName, string contactWebsite, string xmlDocumentationPath)
        {
            //using Swashbuckle.AspNetCore v5
            services.AddSwaggerGen();
            services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>(sp => new SwaggerConfigOptions(sp.GetService<IApiVersionDescriptionProvider>(), apiName, description, contactName, contactWebsite, xmlDocumentationPath));
            return services;
        }
    }

    public class SwaggerConfigOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
        private readonly string _apiName;
        private readonly string _description;
        private readonly string _contactName;
        private readonly string _contactWebsite;
        private readonly string _xmlDocumentationPath;

        public SwaggerConfigOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider, string apiName, string description, string contactName, string contactWebsite, string xmlDocumentationPath)
        {
            _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
            _apiName = apiName;
            _description = description;
            _contactName = contactName;
            _contactWebsite = contactWebsite;
            _xmlDocumentationPath = xmlDocumentationPath;
        }

        public void Configure(SwaggerGenOptions c)
        {
            foreach (var apiDescription in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                c.SwaggerDoc(apiDescription.GroupName, new OpenApiInfo { Title = _apiName, Description = _description, Contact = new OpenApiContact() { Name = _contactName, Email = null, Url = new System.Uri(_contactWebsite) }, Version = apiDescription.ApiVersion.ToString(), License = new OpenApiLicense() { Name = "MIT LICENSE", Url = new System.Uri("https://opensource.org/licenses/MIT") } });
            }

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, SwaggerSecuritySchemes.Bearer);

            c.AddSecurityDefinition(BasicAuthenticationDefaults.AuthenticationScheme, SwaggerSecuritySchemes.Basic);

            //c.AddSecurityDefinition(CookieAuthenticationDefaults.AuthenticationScheme, SwaggerSecuritySchemes.Cookies);

            c.DocInclusionPredicate((documentName, apiDescription) =>
            {
                var actionApiVersionModel = apiDescription.ActionDescriptor
                .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                if (actionApiVersionModel == null)
                {
                    return true;
                }

                if (actionApiVersionModel.DeclaredApiVersions.Any())
                {
                    return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                    $"v{v.ToString()}" == documentName);
                }

                return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                    $"v{v.ToString()}" == documentName);
            });

            c.OperationFilter<SwaggerAssignSecurityRequirements>();
            c.SchemaFilter<SwaggerModelExamples>();

            c.IncludeXmlComments(_xmlDocumentationPath);
            c.DescribeAllEnumsAsStrings();

            c.DescribeAllParametersInCamelCase();
        }
    }
}
