using AspNetCore.Base.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AspNetCore.Base.MultiTenancy.Middleware
{
    //Addition to AppStartup.Configure for configuring Request Pipeline
    //https://andrewlock.net/exploring-istartupfilter-in-asp-net-core/
    public class TenantStartupFilter<TTenant> : IStartupFilter
        where TTenant : AppTenant
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<TenantMiddleware<TTenant>>();
                next(builder);
            };
        }
    }
}
