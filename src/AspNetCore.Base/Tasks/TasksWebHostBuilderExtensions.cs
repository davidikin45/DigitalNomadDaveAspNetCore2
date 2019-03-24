using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.Base.Tasks
{
    //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
    public static class TasksWebHostBuilderExtensions
    {
        private static IWebHostBuilder UseTaskExecutingServer(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices((services) => services.AddTaskExecutingServer());
        }
    }
}
