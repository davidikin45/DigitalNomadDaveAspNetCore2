using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCore.Base.Middleware
{
    public class RequestTasksMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TaskRunnerRequests _taskRunner;

        public RequestTasksMiddleware(RequestDelegate next, TaskRunnerRequests taskRunnerRequests)
        {
            _next = next;
            _taskRunner = taskRunnerRequests;
        }

        public async Task Invoke(HttpContext context)
        {
            await _taskRunner.RunTasksOnEachRequestAsync();

            // Call the next delegate/middleware in the pipeline
            try
            {
                await this._next(context);
            }
            catch
            {
                await _taskRunner.RunTasksOnErrorAsync();
                throw;
            }

            await _taskRunner.RunTasksAfterEachRequestAsync();
        }
    }
}
