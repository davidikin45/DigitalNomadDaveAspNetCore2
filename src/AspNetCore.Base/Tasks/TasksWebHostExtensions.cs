using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.Hosting
{
    public static class TasksWebHostExtensions
    {
        public static async Task InitAsync(this IWebHost host, Func<IServiceProvider, Task> initializeAsync = null)
        {
            using (var scope = host.Services.CreateScope())
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
        }
    }
}
