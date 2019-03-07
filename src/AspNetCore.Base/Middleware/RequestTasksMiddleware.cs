using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.Middleware
{
    public class RequestTasksMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTasksMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TaskRunnerRequests taskRunnerRequests)
        {
            await taskRunnerRequests.RunTasksOnEachRequestAsync();

            // Call the next delegate/middleware in the pipeline
            try
            {
                await this._next(context);
            }
            catch
            {
                await taskRunnerRequests.RunTasksOnErrorAsync();
                throw;
            }

            await taskRunnerRequests.RunTasksAfterEachRequestAsync();
        }
    }
}
