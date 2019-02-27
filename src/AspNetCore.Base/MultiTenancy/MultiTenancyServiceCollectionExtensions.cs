using AspNetCore.Base.Extensions;
using AspNetCore.Base.MultiTenancy.Data.Tenant;
using AspNetCore.Base.MultiTenancy.Data.Tenants;
using AspNetCore.Base.MultiTenancy.Middleware;
using AspNetCore.Base.MultiTenancy.Mvc;
using AspNetCore.Base.MultiTenancy.Request;
using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace AspNetCore.Base.MultiTenancy
{
    public static class MultiTenancyServiceCollectionExtensions
    {
        public static IServiceCollection AddMultiTenancy<TTenant>(this IServiceCollection services, IConfiguration configuration)
            where TTenant : AppTenant
        {
            services.AddTenantService<TTenant>()
                    .AddTenantStrategyService<TTenant>()
                    .AddTenantMiddleware<TTenant>()
                    .AddTenantLocations<TTenant>()
                    .AddTenantRequestIdentification<TTenant>().TenantForHostQueryStringSourceIP()
                    .AddTenantConfiguration<TTenant>(Assembly.GetCallingAssembly())
                    .AddTenantSettings<TTenant>(configuration);

            return services;
        }

        public static IServiceCollection AddMultiTenancyDbContextStoreInMemory<TTenantStore, TTenant>(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
          where TTenantStore : DbContext, ITenantsStore<TTenant>
          where TTenant : AppTenant
        {
            return services.AddMultiTenancyDbContextStore<TTenantStore, TTenant>("", contextLifetime);
        }

        public static IServiceCollection AddMultiTenancyDbContextStore<TTenantStore, TTenant>(this IServiceCollection services, string connectionString, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
           where TTenantStore : DbContext, ITenantsStore<TTenant>
            where TTenant : AppTenant
        {
            services.AddDbContext<TTenantStore>(connectionString, contextLifetime);
            services.Add(new ServiceDescriptor(typeof(ITenantsStore<TTenant>), sp => sp.GetRequiredService<TTenantStore>(), contextLifetime));
            return services;
        }

        public static IServiceCollection AddMultiTenancyStore<TTenantStore, TTenant>(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
           where TTenantStore : class, ITenantsStore<TTenant>
            where TTenant : AppTenant
        {
            services.Add(new ServiceDescriptor(typeof(TTenantStore), typeof(TTenantStore), contextLifetime));
            services.Add(new ServiceDescriptor(typeof(ITenantsStore<TTenant>), sp => sp.GetRequiredService<TTenantStore>(), contextLifetime));
            return services;
        }

        public static IServiceCollection AddTenantSettings<TTenant>(this IServiceCollection services, IConfiguration configuration)
            where TTenant : AppTenant
        {
            services.Configure<TenantSettings<TTenant>>(configuration.GetSection("TenantSettings"));
            return services.AddTransient(sp => sp.GetService<IOptions<TenantSettings<TTenant>>>().Value);
        }

        public static IServiceCollection AddTenantService<TTenant>(this IServiceCollection services)
            where TTenant : AppTenant
        {
            return services
                .AddHttpContextAccessor()
                .AddScoped<MultiTenantService<TTenant>>()
                .AddScoped<ITenantService>(x => x.GetRequiredService<MultiTenantService<TTenant>>())
                .AddScoped<ITenantService<TTenant>>(x => x.GetRequiredService<MultiTenantService<TTenant>>())
                .AddScoped(sp => sp.GetService<ITenantService<TTenant>>().GetTenant());
        }

        public static IServiceCollection AddTenantStrategyService<TTenant>(this IServiceCollection services)
            where TTenant : AppTenant
        {
            return services.AddSingleton<ITenantDbContextStrategyService, MultiTenantDbContextStrategyService>();
        }

        public static TenantRequestIdentification<TTenant> AddTenantRequestIdentification<TTenant>(this IServiceCollection services)
            where TTenant : AppTenant
        {
            return new TenantRequestIdentification<TTenant>(services);
        }

        public static TenantDbContextIdentification<TContext> AddTenantDbContextIdentification<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return new TenantDbContextIdentification<TContext>(services);
        }

        public static IServiceCollection AddTenantLocations<TTenant>(this IServiceCollection services)
             where TTenant : AppTenant
        {
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                if (!(options.ViewLocationExpanders.FirstOrDefault() is TenantViewLocationExpander<TTenant>))
                {
                    options.ViewLocationExpanders.Insert(0, new TenantViewLocationExpander<TTenant>());
                }
            });
        }

        public static IServiceCollection AddTenantConfiguration<TTenant>(this IServiceCollection services, Assembly assembly)
            where TTenant : AppTenant
        {
            var types = assembly
                .GetExportedTypes()
                .Where(type => typeof(ITenantConfiguration).IsAssignableFrom(type))
                .Where(type => (type.IsAbstract == false) && (type.IsInterface == false));

            foreach (var type in types)
            {
                services.AddSingleton(typeof(ITenantConfiguration), type);
            }

            return services;
        }

        public static IServiceCollection AddTenantConfiguration<TTenant>(this IServiceCollection services)
            where TTenant : AppTenant
        {
            var target = Assembly.GetCallingAssembly();
            return services.AddTenantConfiguration<TTenant>(target);
        }

        public static IServiceCollection AddTenantMiddleware<TTenant>(this IServiceCollection services)
           where TTenant : AppTenant
        {
            return services.AddSingleton<IStartupFilter, TenantStartupFilter<TTenant>>();
        }
    }
}
