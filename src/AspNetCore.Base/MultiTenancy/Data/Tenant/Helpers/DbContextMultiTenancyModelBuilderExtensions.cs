using AspNetCore.Base.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AspNetCore.Base.MultiTenancy.Data.Tenant.Helpers
{
    public static class DbContextMultiTenancyModelBuilderExtensions
    {
        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(string));

        private static LambdaExpression IsTenantRestriction(Type type, string tenantId, string tenantPropertyName)
        {
            var parm = Expression.Parameter(type, "p");
            var prop = Expression.Call(_propertyMethod, parm, Expression.Constant(tenantPropertyName));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(tenantId));
            var lambda = Expression.Lambda(condition, parm);

            return lambda;
        }

        public static void AddTenantSchema(this ModelBuilder modelBuilder, string tenantId, bool selectGenericInterface = false)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => typeof(IEntityTenantSchema).IsAssignableFrom(x.ClrType) || (selectGenericInterface && typeof(IEntityTenant).IsAssignableFrom(x.ClrType))))
            {
                entityType.Relational().Schema = tenantId;
            }
        }

        public static void AddTenantFilter(this ModelBuilder modelBuilder, string tenantId, string tenantPropertyName = "TenantId")
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tenantProperty = entityType.FindProperty(tenantPropertyName);
                if (tenantProperty != null && tenantProperty.ClrType == typeof(string))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(IsTenantRestriction(entityType.ClrType, tenantId, tenantPropertyName));
                }
            }
        }

        public static void AddTenantShadowPropertyFilter(this ModelBuilder modelBuilder, string tenantId, string tenantPropertyName = "TenantId", bool selectGenericInterface = false)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => typeof(IEntityTenantFilterShadowProperty).IsAssignableFrom(x.ClrType) || (selectGenericInterface && typeof(IEntityTenant).IsAssignableFrom(x.ClrType))))
            {
                entityType.AddProperty(tenantPropertyName, typeof(string));
                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasQueryFilter(IsTenantRestriction(entityType.ClrType, tenantId, tenantPropertyName));
            }
        }
    }
}
