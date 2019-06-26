using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

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

            //Replace IISServerSetupFilter
            var iisServerSetupFilter = services.FirstOrDefault(s => s.ImplementationInstance != null && s.ImplementationInstance.GetType().Name == "IISServerSetupFilter");
            if (iisServerSetupFilter != null)
            {
                var virtualPath = (string)iisServerSetupFilter.ImplementationInstance.GetType().GetField("_virtualPath", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(iisServerSetupFilter.ImplementationInstance);
                var index = services.IndexOf(iisServerSetupFilter);
                services[index] = ServiceDescriptor.Singleton(typeof(IStartupFilter), new TaskExecutingServerIISServerSetupFilter(virtualPath));
            }

            // Decorate the IServer with our TaskExecutingServer
            return services.Decorate<IServer, TaskExecutingServer>();
        }
    }
}
