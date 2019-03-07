using AspNetCore.Base.Data.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Base.MultiTenancy.Data.Tenants.Initializers
{
    public static class TenantContenInitializerHelper
    {
        public async static Task InitializeContextsAsync<TDbContextTenants, TTenant>(TDbContextTenants context, Dictionary<Type,Type> contextInitializers, IServiceProvider serviceProvider)
        where TDbContextTenants : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
        {
            foreach (var contextInitializer in contextInitializers)
            {
                var migrator = Activator.CreateInstance(contextInitializer.Value);

                var connectionStrings = new HashSet<string>();
                foreach (var tenant in context.Tenants)
                {

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService<TTenant>>();
                        tenantService.SetTenant(tenant);

                        using (var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(contextInitializer.Key))
                        {
                            var connectionString = tenant.GetConnectionString(tenantService.GetTenantStrategy(dbContext).ConnectionStringName);

                            if (connectionString == null)
                            {
                                connectionString = "__DEFAULT";
                            }

                            var genericType = typeof(IDbContextInitializer<>).MakeGenericType(contextInitializer.Key);

                            if (!connectionStrings.Contains(connectionString))
                            {
                                await (Task)genericType.GetMethod(nameof(IDbContextInitializer<DbContext>.InitializeSchemaAsync)).Invoke(migrator, new object[] { dbContext });
                                connectionStrings.Add(connectionString);
                            }

                            var result = (Task)genericType.GetMethod(nameof(IDbContextInitializer<DbContext>.InitializeDataAsync)).Invoke(migrator, new object[] { dbContext, tenant.Id });
                            await result;
                        }
                    }
                }
            }
        }
    }
}

