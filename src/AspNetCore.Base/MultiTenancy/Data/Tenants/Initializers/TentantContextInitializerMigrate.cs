using AspnetCore.Base.Data.Initializers;
using AspNetCore.Base.Data.Initializers;
using AspNetCore.Base.MultiTenancy;
using AspNetCore.Base.MultiTenancy.Data.Tenant;
using AspNetCore.Base.MultiTenancy.Data.Tenants;
using AspNetCore.Base.MultiTenancy.Data.Tenants.Initializers;
using AspNetCore.Base.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCore.Base.Data.Tenants.Initializers
{
    public abstract class TenantsContextInitializerMigrate<TDbContextTenants, TTenant> : ContextInitializerMigrate<TDbContextTenants>
        where TDbContextTenants : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private Dictionary<Type, Type> _contextInitializers = new Dictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;
        protected readonly TenantSettings<TTenant> settings;
        public TenantsContextInitializerMigrate(IServiceProvider serviceProvider, TenantSettings<TTenant> settings)
        {
            _serviceProvider = serviceProvider;
            this.settings = settings;
        }

        public void AddContextInitializer<TDbContext, TInitializer>()
             where TDbContext : DbContext
             where TInitializer : IDbContextInitializer<TDbContext>
        {
            _contextInitializers.Add(typeof(TDbContext), typeof(TDbContext));
        }

        public async override Task OnSeedCompleteAsync(TDbContextTenants context)
        {
            await TenantContenInitializerHelper.InitializeContextsAsync<TDbContextTenants, TTenant>(context, _contextInitializers, _serviceProvider);
        }
    }
}
