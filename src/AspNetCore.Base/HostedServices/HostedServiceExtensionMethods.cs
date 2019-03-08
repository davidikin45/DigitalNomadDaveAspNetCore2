using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Base.HostedServices
{
    public static class HostedServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddHostedServiceCronJob<TCronJob>(this IServiceCollection services, params string[] cronSchedules)
            where TCronJob : class, IHostedServiceCronJob
        {
            services.AddScoped<TCronJob>();

            return services.AddTransient<IHostedService>(sp =>
            {
                var logger = sp.GetService<ILogger<HostedServiceCron<TCronJob>>>();
                return new HostedServiceCron<TCronJob>(sp, logger, cronSchedules);
            });
        }
    }
}
