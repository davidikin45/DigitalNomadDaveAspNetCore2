using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    //This allows initialization tasks to run AFTER IStartupFilters have run and the middleware pipeline has been configured.
    public class TaskExecutingServer : IServer
    {
        // Inject the original IServer implementation (KestrelServer/IISHttpServer)
        internal IServer Server {get;}
        private readonly IServiceProvider _serviceProvider;
        public TaskExecutingServer(IServer server, IServiceProvider serviceProvider)
        {
             Server = server;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            // Run the tasks first
            await _serviceProvider.InitAsync();

            // Now start the Kestrel server properly
            await Server.StartAsync(application, cancellationToken);
        }

        // Delegate implementation to default IServer
        public IFeatureCollection Features => Server.Features;
        public void Dispose() => Server.Dispose();
        public Task StopAsync(CancellationToken cancellationToken) => Server.StopAsync(cancellationToken);
    }
}
