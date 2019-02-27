using AspNetCore.Base.MultiTenancy.Hangfire;
using Hangfire.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace AspNetCore.Base.MultiTenancy
{
    public static class HangfireMultiTenantExtensions
    {
        public static IApplicationBuilder UseHangfireDashboardMultiTenant(
            [NotNull] this IApplicationBuilder app,
            [NotNull] string route = "/admin/hangfire")
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (route == null) throw new ArgumentNullException(nameof(route));

            app.Map(new PathString(route), x => x.UseMiddleware<AspNetCoreMultiTenantDashboardMiddleware>(route));

            return app;
        }
    }
}
