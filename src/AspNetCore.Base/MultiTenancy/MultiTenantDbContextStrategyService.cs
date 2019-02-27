using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public class MultiTenantDbContextStrategyService : ITenantDbContextStrategyService
    {
        private readonly IServiceProvider _serviceProvider;
        public MultiTenantDbContextStrategyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDbContextTenantStrategy GetStrategy(DbContext context)
        {
            var dbContextType = context.GetType();
            var dbContextStrategyType = typeof(IDbContextTenantStrategy<>).MakeGenericType(dbContextType);
            return (IDbContextTenantStrategy)_serviceProvider.GetService(dbContextStrategyType);
        }

    }
}
