using AspNetCore.Base.Data.Helpers;
using AspNetCore.Base.Helpers;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AspNetCore.Base.Hangfire
{
    public static class HangfireConfigurationExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return services.AddHangfireInMemory();
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                return services.AddHangfireSqlLite(connectionString, initializeDatabase);
            }
            else
            {
                return services.AddHangfireSqlServer(connectionString, initializeDatabase);
            }
        }

        public static IServiceCollection AddHangfireInMemory(this IServiceCollection services)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseFilter(new HangfirePreserveOriginalQueueAttribute());

                var options = new MemoryStorageOptions
                {

                };

                config.UseMemoryStorage(options);
            });
        }

        public static IServiceCollection AddHangfireSqlServer(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseFilter(new HangfirePreserveOriginalQueueAttribute());
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = initializeDatabase,
                    QueuePollInterval = TimeSpan.FromSeconds(15) // Default value
                };

                //Initializes Hangfire Schema if PrepareSchemaIfNecessary = true
                config.UseSqlServerStorage(connectionString, options);
            });
        }

        public static IServiceCollection AddHangfireSqlLite(this IServiceCollection services, string connectionString, bool initializeDatabase)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseFilter(new HangfirePreserveOriginalQueueAttribute());

                var options = new SQLiteStorageOptions
                {
                    PrepareSchemaIfNecessary = initializeDatabase,
                    QueuePollInterval = TimeSpan.FromSeconds(15) // Default value
                };

                //Initializes Hangfire Schema if PrepareSchemaIfNecessary = true
                config.UseSQLiteStorage(connectionString, options);
            });
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder builder, string serverName, string route = "/admin/hangfire")
        {
            builder.UseHangfireDashboard(route, new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationfilter() },
                AppPath = route.Replace("/hangfire", "")
            });

            //each microserver has its own queue. Queue by using the Queue attribute.
            //https://discuss.hangfire.io/t/one-queue-for-the-whole-farm-and-one-queue-by-server/490
            var options = new BackgroundJobServerOptions
            {
                ServerName = serverName,
                Queues = new[] { serverName, "default" }
            };

            //https://discuss.hangfire.io/t/one-queue-for-the-whole-farm-and-one-queue-by-server/490/3

            builder.UseHangfireServer(options);
            return builder;
        }

        public static IServiceCollection AddHangfireJob<HangfireJob>(this IServiceCollection services)
            where HangfireJob : class
        {
            return services.AddTransient<HangfireJob>();
        }
    }
}
