using AspNetCore.Base;
using AspNetCore.Base.Cqrs;
using AspNetCore.Base.DomainEvents;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.IntegrationEvents;
using AspNetCore.Base.Tasks;
using DND.ApplicationServices;
using DND.Core;
using DND.Data;
using DND.Data.Identity;
using DND.Domain.Identity;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

//[assembly: ApiController]
//[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace DND.Web
{
    public class Startup : AppStartupMvcIdentity<IdentityContext, User>
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, configuration, hostingEnvironment)
        {

        }

        public override void AddDatabases(IServiceCollection services, string tenantsConnectionString, string identityConnectionString, string hangfireConnectionString, string defaultConnectionString)
        {
            services.AddDbContext<AppContext>(defaultConnectionString);
            services.AddDbContext<IdentityContext>(identityConnectionString);
        }

        public override void AddUnitOfWorks(IServiceCollection services)
        {
            services.AddUnitOfWork<IAppUnitOfWork, AppUnitOfWork>();
        }

        public override void AddHostedServices(IServiceCollection services)
        {
            //services.AddHostedServiceCronJob<Job2>("* * * * *");
        }

        public override void AddHangfireJobServices(IServiceCollection services)
        {
         
            //services.AddHangfireJob<Job1>();
        }

        public override void AddHttpClients(IServiceCollection services)
        {

        }
    }

    public class HangfireScheduledJobs : IAsyncInitializer
    {
        private readonly IRecurringJobManager _recurringJobManager;
        public HangfireScheduledJobs(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public Task ExecuteAsync()
        {
            //_recurringJobManager.AddOrUpdate("check-link", Job.FromExpression<Job1>(m => m.Execute()), Cron.Minutely(), new RecurringJobOptions());
            //_recurringJobManager.Trigger("check-link");

            return Task.CompletedTask;
        }
    }

    public class CqrsCommands : IAsyncInitializer
    {
        private readonly ICqrsMediator _mediator;
        public CqrsCommands(ICqrsMediator mediator)
        {
            _mediator = mediator;
        }

        public Task ExecuteAsync()
        {
            _mediator.CqrsCommandSubscriptionManager.AddDynamicSubscription<dynamic, object, CommandHandler>("*");
            return Task.CompletedTask;
        }
    }

    public class CqrsQueries : IAsyncInitializer
    {
        private readonly ICqrsMediator _mediator;
        public CqrsQueries(ICqrsMediator mediator)
        {
            _mediator = mediator;
        }

        public Task ExecuteAsync()
        {
            _mediator.CqrsQuerySubscriptionManager.AddDynamicSubscription<dynamic, object, QueryHandler>("*");
            return Task.CompletedTask;
        }
    }

    public class DomainEvents : IAsyncInitializer
    {
        private readonly IDomainEventBus _eventBus;
        public DomainEvents(IDomainEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task ExecuteAsync()
        {
            _eventBus.DomainEventSubscriptionsManager.AddDynamicSubscription<dynamic, DomainEventHandler>("*");
            return Task.CompletedTask;
        }
    }

    public class IntegrationEvents : IAsyncInitializer
    {
        private readonly IIntegrationEventBus _eventBus;
        public IntegrationEvents(IIntegrationEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
