using AspNetCore.Base.Hangfire;
using Autofac.Multitenant;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.Base.MultiTenancy.Hangfire
{
    public static class HangfireMultiTenantHelper
    {
        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServer(
            string tenantId,
            string serverName,
            string connectionString,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            MultitenantContainer mtc,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses
            )
        {
            var tenantJobActivator = new AspNetCoreMultiTenantJobActivator(mtc, tenantId);

            return HangfireHelper.StartHangfireServer(
                serverName,
                connectionString,
                applicationLifetime,
                jobFilters,
                tenantJobActivator,
                backgroundJobFactory,
                backgroundJobPerformer,
                backgroundJobStateChanger,
                additionalProcesses);
        }
    }
}
