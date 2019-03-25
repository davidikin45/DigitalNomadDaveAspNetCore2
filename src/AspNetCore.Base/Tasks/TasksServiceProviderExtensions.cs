using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public static class TasksServiceProviderExtensions
    {
        public static async Task InitAsync(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                if (scope.ServiceProvider.GetService<TaskRunnerDbInitialization>() != null)
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<TaskRunnerDbInitialization>();
                    await taskRunner.RunDbInitializationTasksAsync();
                }

                if (scope.ServiceProvider.GetService<TaskRunnerInitialization>() != null)
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<TaskRunnerInitialization>();
                    await taskRunner.RunInitializationTasksAsync();
                }
            }
        }
    }
}
