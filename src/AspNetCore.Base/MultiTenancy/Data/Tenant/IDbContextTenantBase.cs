namespace AspNetCore.Base.MultiTenancy.Data.Tenant
{
    interface IDbContextTenantBase
    {
         ITenantService TenantService { get; }
    }
}
