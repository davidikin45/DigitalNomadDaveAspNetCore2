using AspNetCore.Base.Helpers;
using Microsoft.Extensions.DependencyInjection;
using MiniProfiler.Initialization;
using StackExchange.Profiling.Storage;

namespace AspNetCore.Base.MiniProfiler
{
    public static class MiniProfileConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddMiniProfiler(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return services.AddMiniProfilerInMemory();
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                return services.AddMiniProfilerSqlLite(connectionString, initializeDatabase);
            }
            else
            {
                return services.AddMiniProfilerSqlServer(connectionString, initializeDatabase);
            }
        }

        public static IServiceCollection AddMiniProfilerInMemory(this IServiceCollection services)
        {
            services.AddMiniProfiler(options => {
                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;

                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupStartHidden = true; //ALT + P to display
                options.PopupShowTrivial = true;
                options.PopupShowTimeWithChildren = true;
                options.ResultsAuthorize = (request) => true;
                options.UserIdProvider = (request) => request.HttpContext.User.Identity.Name;
            }).AddEntityFramework();

            return services;
        }

        public static IServiceCollection AddMiniProfilerSqlServer(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
             services.AddMiniProfiler(options => {
                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;

                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupStartHidden = true; //ALT + P to display
                options.PopupShowTrivial = true;
                options.PopupShowTimeWithChildren = true;
                options.ResultsAuthorize = (request) => true;
                options.UserIdProvider = (request) => request.HttpContext.User.Identity.Name;

                options.Storage = new SqlServerStorage(connectionString);
            }).AddEntityFramework();

            if (initializeDatabase)
            {
                MiniProfilerInitializer.EnsureDbAndTablesCreatedAsync(connectionString).Wait();
            }

            return services;
        }

        public static IServiceCollection AddMiniProfilerSqlLite(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
            services.AddMiniProfiler(options => {
                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;

                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupStartHidden = true; //ALT + P to display
                options.PopupShowTrivial = true;
                options.PopupShowTimeWithChildren = true;
                options.ResultsAuthorize = (request) => true;
                options.UserIdProvider = (request) => request.HttpContext.User.Identity.Name;

                options.Storage = new SqliteStorage(connectionString);
            }).AddEntityFramework();

            if(initializeDatabase)
            {
                MiniProfilerInitializer.EnsureDbAndTablesCreatedAsync(connectionString).Wait();
            }

            return services;
        }
    }
}
