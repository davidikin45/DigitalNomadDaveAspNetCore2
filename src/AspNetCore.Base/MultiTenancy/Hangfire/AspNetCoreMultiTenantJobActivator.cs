using Autofac;
using Autofac.Multitenant;
using Hangfire;
using Hangfire.Annotations;
using System;

namespace AspNetCore.Base.MultiTenancy.Hangfire
{
    public class AspNetCoreMultiTenantJobActivator : JobActivator
    {
        private readonly ILifetimeScope _serviceScopeFactory;
        private readonly object _tenantId;

        public AspNetCoreMultiTenantJobActivator(MultitenantContainer mtc, object tenantId)
        {
            _serviceScopeFactory = mtc.GetTenantScope(tenantId);
            _tenantId = tenantId;
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            var scope = _serviceScopeFactory.BeginLifetimeScope();
            var tenantService = scope.Resolve<ITenantService>();
            tenantService.SetTenantByIdAsync(_tenantId);
            return new AspNetCoreMultiTenantJobActivatorScope(scope);
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override JobActivatorScope BeginScope()
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            var scope = _serviceScopeFactory.BeginLifetimeScope();
            var tenantService = scope.Resolve<ITenantService>();
            tenantService.SetTenantByIdAsync(_tenantId);
            return new AspNetCoreMultiTenantJobActivatorScope(scope);
        }

        public override object ActivateJob(Type jobType)
        {
            return base.ActivateJob(jobType);
        }
    }

    public class AspNetCoreMultiTenantJobActivatorScope : JobActivatorScope
    {
        private readonly ILifetimeScope _serviceScope;

        public AspNetCoreMultiTenantJobActivatorScope([NotNull] ILifetimeScope serviceScope)
        {
            if (serviceScope == null) throw new ArgumentNullException(nameof(serviceScope));
            _serviceScope = serviceScope;
        }

        public override object Resolve(Type type)
        {
            return _serviceScope.Resolve(type);
        }

        public override void DisposeScope()
        {
            _serviceScope.Dispose();
        }
    }
}


