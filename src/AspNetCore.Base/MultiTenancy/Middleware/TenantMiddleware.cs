using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Middleware
{
    public class TenantMiddleware<TTenant>
        where TTenant : AppTenant
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context.Items.ContainsKey("_tenantMiddleware") == false)
            {
                var service = context.RequestServices.GetService<ITenantService<TTenant>>();
                var tenant = service.GetTenant();
                if(tenant != null)
                {
                    var configuration = context.RequestServices.GetService<IConfiguration>();
                    var environment = context.RequestServices.GetService<IHostingEnvironment>();
                    var providers = (configuration as ConfigurationRoot).Providers as List<IConfigurationProvider>;

                    var tenatProviders = providers.OfType<TenantJsonConfigurationProvider>().ToList();
                    if (tenatProviders.Count == 0)
                    {
                        //var tenantProviders = (TenantConfig.BuildTenantConfiguration(environment, tenant.Id) as ConfigurationRoot).Providers as List<IConfigurationProvider>;
                        //providers.Insert(2, tenantProviders[0]);
                        //providers.Insert(4, tenantProviders[1]);
                    }
                    else
                    {
                        //var tenantProviders = (TenantConfig.BuildTenantConfiguration(environment, tenant.Id) as ConfigurationRoot).Providers as List<IConfigurationProvider>;
                        //providers[2] = tenantProviders[0];
                        //providers[4] = tenantProviders[1];
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Task.CompletedTask;
                }
                context.Items["_tenantMiddleware"] = tenant;
            }

            return this._next(context);
        }
    }
}
