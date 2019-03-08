using AspNetCore.Base.Data.UnitOfWork;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.MultiTenancy;
using AspNetCore.Base.MultiTenancy.Data.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Base.Extensions
{
    public static class DbServiceCollectionExtensions
    {
        public static TenantDbContextIdentification<TContext> AddDbContextTenant<TContext>(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContextTenant<TContext>(null, contextLifetime);
        }

        public static TenantDbContextIdentification<TContext> AddDbContextTenant<TContext>(this IServiceCollection services, string defaultConnectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(defaultConnectionString, contextLifetime).AddTenantDbContextIdentification<TContext>();
        }

        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    {
                        options.SetConnectionString<TContext>(connectionString);
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    }, contextLifetime);
        }

        public static DbContextOptionsBuilder SetConnectionString<TContext>(this DbContextOptionsBuilder options, string connectionString, string migrationsAssembly = "")
            where TContext : DbContext
        {
            if (connectionString == null)
            {
                return options;
            }
            else if (connectionString == string.Empty)
            {
                return options.UseInMemoryDatabase(typeof(TContext).FullName);
            }
            if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                if(!string.IsNullOrWhiteSpace(migrationsAssembly))
                {
                    return options.UseSqlite(connectionString, sqlOptions => {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.UseNetTopologySuite();
                    });
                }
                return options.UseSqlite(connectionString, sqlOptions => {
                    sqlOptions.UseNetTopologySuite();
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                {
                    return options.UseSqlServer(connectionString, sqlOptions => {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.UseNetTopologySuite();
                        });
                }
                return options.UseSqlServer(connectionString, sqlOptions => {
                    sqlOptions.UseNetTopologySuite();
                });
            }
        }

        //https://medium.com/volosoft/asp-net-core-dependency-injection-best-practices-tips-tricks-c6e9c67f9d96
        public static IServiceCollection AddDbContextInMemory<TContext>(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()), contextLifetime);
        }

        public static IServiceCollection AddDbContextSqlServer<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString, sqlOptions => {
                        sqlOptions.UseNetTopologySuite();
                    }), contextLifetime);
        }

        public static IServiceCollection AddDbContextSqlite<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContext<TContext>(options =>
                    options.UseSqlite(connectionString, sqlOptions => {
                        sqlOptions.UseNetTopologySuite();
                    }), contextLifetime);
        }

        public static IServiceCollection AddDbContextPoolSqlServer<TContext>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            return services.AddDbContextPool<TContext>(options =>
                    options.UseSqlServer(connectionString, sqlOptions => {
                        sqlOptions.UseNetTopologySuite();
                    }));
        }

        public static IServiceCollection AddDbContextSqlServerWithRetries<TContext>(this IServiceCollection services, string connectionString, int retries = 10, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
           return services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: retries,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                        sqlOptions.UseNetTopologySuite();
                    }), contextLifetime);
        }

        public static void AddUnitOfWork<TUnitOfWorkImplementation>(this IServiceCollection services)
        where TUnitOfWorkImplementation : UnitOfWorkBase
        {
            services.AddScoped<TUnitOfWorkImplementation>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<TUnitOfWorkImplementation>());
        }

        public static void AddUnitOfWork<TUnitOfWork, TUnitOfWorkImplementation>(this IServiceCollection services)
            where TUnitOfWork : class
            where TUnitOfWorkImplementation : UnitOfWorkBase, TUnitOfWork
        {
            services.AddScoped<TUnitOfWorkImplementation>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<TUnitOfWorkImplementation>());
            services.AddScoped<TUnitOfWork>(sp => sp.GetService<TUnitOfWorkImplementation>());
        }
    }
}
