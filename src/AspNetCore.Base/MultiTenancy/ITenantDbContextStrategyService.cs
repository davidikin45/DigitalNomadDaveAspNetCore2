using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    public interface ITenantDbContextStrategyService
    {
        IDbContextTenantStrategy GetStrategy(DbContext context);
    }
}
