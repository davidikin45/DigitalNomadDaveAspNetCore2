using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace AspNetCore.Base.Hosting
{
    public static class TasksWebHostExtensions
    {
        public static Task InitAsync(this IWebHost host)
        {
            return host.Services.InitAsync();
        }
    }
}
