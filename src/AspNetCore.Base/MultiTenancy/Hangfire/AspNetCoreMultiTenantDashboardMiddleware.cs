using AspNetCore.Base.Hangfire;
using AspNetCore.Base.Helpers;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Hangfire
{
    public class AspNetCoreMultiTenantDashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;
        private readonly RouteCollection _routes;
        private readonly string _route;

        public AspNetCoreMultiTenantDashboardMiddleware(RequestDelegate next, IConfiguration configration, ITenantService tenantService, RouteCollection routes, string route)
        {
            _next = next;
            _configuration = configration;
            _tenantService = tenantService;
            _routes = routes;
            _route = route;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenant = (AppTenant)context.Items["_tenant"];

            var connectionString = tenant.GetConnectionString("HangfireConnection") ?? (_configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "HangfireConnection") ? _configuration.GetConnectionString("HangfireConnection") : null);
            if(connectionString != null)
            {
                JobStorage storage;
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    storage = new MemoryStorage();
                }
                if (ConnectionStringHelper.IsSQLite(connectionString))
                {
                    storage = new SQLiteStorage(connectionString);
                }
                else
                {
                    storage = new SqlServerStorage(connectionString);
                }

                var options = new DashboardOptions
                {
                    Authorization = new[] { new HangfireAuthorizationfilter() },
                    AppPath = _route.Replace("/hangfire", "")
                };

                var middleware = new AspNetCoreDashboardMiddleware(_next, storage, options, _routes);
                await middleware.Invoke(context);
            }

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}