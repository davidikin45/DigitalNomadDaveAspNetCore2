using AspnetCore.Base.Data.Initializers;
using AspNetCore.Base.Data.Initializers;
using AspNetCore.Base.MultiTenancy;
using AspNetCore.Base.MultiTenancy.Data.Tenants;
using AspNetCore.Base.MultiTenancy.Data.Tenants.Initializers;
using AspNetCore.Base.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCore.Base.Data.Tenants.Initializers
{
    public abstract class TenantsContextInitializerDropCreate<TDbContextTenants, TTenant> : ContextInitializerDropCreate<TDbContextTenants>
        where TDbContextTenants : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private Dictionary<Type, Type> _contextInitializers = new Dictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;
        protected readonly TenantSettings<TTenant> settings;
        public TenantsContextInitializerDropCreate(IServiceProvider serviceProvider, TenantSettings<TTenant> settings)
        {
            _serviceProvider = serviceProvider;
            this.settings = settings;
        }

        public void AddContextInitializer<TDbContext, TInitializer>()
             where TDbContext : DbContext
             where TInitializer : IDbContextInitializer<TDbContext>
        {
            _contextInitializers.Add(typeof(TDbContext), typeof(TInitializer));
        }

        public async override Task OnSeedCompleteAsync(TDbContextTenants context)
        {
            await TenantContenInitializerHelper.InitializeContextsAsync<TDbContextTenants, TTenant>(context, _contextInitializers, _serviceProvider);
        }
    }
}
