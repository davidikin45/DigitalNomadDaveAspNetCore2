using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
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
            using (var scope = _serviceProvider.CreateScope())
            {
                if (scope.ServiceProvider.GetService<TaskRunnerDbInitialization>() != null)
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<TaskRunnerDbInitialization>();
                    await taskRunner.RunTasksAfterApplicationConfigurationAsync();
                }

                if (scope.ServiceProvider.GetService<TaskRunnerInitialization>() != null)
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<TaskRunnerInitialization>();
                    await taskRunner.RunTasksAfterApplicationConfigurationAsync();
                }
            }

            // Now start the Kestrel server properly
            await _server.StartAsync(application, cancellationToken);
        }

        // Delegate implementation to default IServer
        public IFeatureCollection Features => _server.Features;
        public void Dispose() => _server.Dispose();
        public Task StopAsync(CancellationToken cancellationToken) => _server.StopAsync(cancellationToken);
    }
}
