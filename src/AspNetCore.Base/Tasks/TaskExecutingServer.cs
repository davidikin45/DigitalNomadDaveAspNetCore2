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
        // Inject the original IServer implementation (KestrelServer)
        private readonly IServer _server;
        private readonly IServiceProvider _serviceProvider;
        public TaskExecutingServer(IServer server, IServiceProvider serviceProvider)
        {
            _server = server;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            // Run the tasks first
            await _serviceProvider.InitAsync();

            // Now start the Kestrel server properly
            await _server.StartAsync(application, cancellationToken);
        }

        // Delegate implementation to default IServer
        public IFeatureCollection Features => _server.Features;
        public void Dispose() => _server.Dispose();
        public Task StopAsync(CancellationToken cancellationToken) => _server.StopAsync(cancellationToken);
    }
}
