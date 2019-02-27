using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCore.Base.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string apiName, string description, string contactName, string contactWebsite, string version, string xmlDocumentationPath)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new Info { Title = apiName, Description = description, Contact = new Contact() { Name = contactName, Email = null, Url = contactWebsite }, Version = version });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.OperationFilter<SwaggerAssignSecurityRequirements>();
                c.SchemaFilter<SwaggerModelExamples>();

                c.IncludeXmlComments(xmlDocumentationPath);
                c.DescribeAllEnumsAsStrings();

                c.DescribeAllParametersInCamelCase();
            });

            return services;
        }
    }
}
