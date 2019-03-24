using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AspNetCore.Base.Tasks
{
    public static class TasksServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskExecutingServer(this IServiceCollection services)
        {
            var decoratorType = typeof(TaskExecutingServer);
            if (services.Any(service => service.ImplementationType == decoratorType))
            {
                // We've already decorated the IServer
                return services;
            }

            // Decorate the IServer with our TaskExecutingServer
            return services.Decorate<IServer, TaskExecutingServer>();
        }
    }
}
