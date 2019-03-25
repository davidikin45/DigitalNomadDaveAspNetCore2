using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Tasks
{
    public class TaskRunnerAfterApplicationConfiguration
    {
        public IEnumerable<IRunAfterApplicationConfiguration> RunAfterApplicationConfiguration { get; set; }
        public TaskRunnerAfterApplicationConfiguration()
        {
        }

        public void RunTasksAfterApplicationConfiguration()
        {
            foreach (IRunAfterApplicationConfiguration task in RunAfterApplicationConfiguration)
            {
                task.Execute();
            }
        }
    }

    public class TaskRunnerDbInitialization
    {
        private readonly ILogger<TaskRunnerDbInitialization> _logger;
        public IEnumerable<IAsyncDbInitializer> AsyncInitializers { get; set; }

        public TaskRunnerDbInitialization(ILogger<TaskRunnerDbInitialization> logger)
        {
            _logger = logger;
        }

        public async Task RunDbInitializationTasksAsync()
        {
            _logger.LogInformation("Starting async db initialization");

            try
            {
                foreach (IAsyncDbInitializer task in AsyncInitializers.ToList().OrderBy(i => i.Order))
                {
                    _logger.LogInformation("Starting async initialization for {InitializerType}", task.GetType());
                    try
                    {
                        await task.ExecuteAsync();
                        _logger.LogInformation("Async initialization for {InitializerType} completed", task.GetType());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Async initialization for {InitializerType} failed", task.GetType());
                        throw;
                    }
                }

                _logger.LogInformation("Async db initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Async db initialization failed");
                throw;
            }
        }
    }

    public class TaskRunnerInitialization
    {
        private readonly ILogger<TaskRunnerInitialization> _logger;
        public IEnumerable<IAsyncInitializer> AsyncInitializers { get; set; }

        public TaskRunnerInitialization(ILogger<TaskRunnerInitialization> logger)
        {
            _logger = logger;
        }

        public async Task RunInitializationTasksAsync()
        {
            _logger.LogInformation("Starting async initialization");

            try
            {
                foreach (IAsyncInitializer task in AsyncInitializers)
                {
                    _logger.LogInformation("Starting async initialization for {InitializerType}", task.GetType());
                    try
                    {
                        await task.ExecuteAsync();
                        _logger.LogInformation("Async initialization for {InitializerType} completed", task.GetType());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Async initialization for {InitializerType} failed", task.GetType());
                        throw;
                    }
                }

                _logger.LogInformation("Async initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Async initialization failed");
                throw;
            }
        }
    }


    public class TaskRunnerRequests
    {
        public IEnumerable<IRunOnEachRequest> RunOnEachRequest { get; set; }
        public IEnumerable<IRunAfterEachRequest> RunAfterEachRequest { get; set; }
        public IEnumerable<IRunOnError> RunOnError { get; set; }

        public TaskRunnerRequests()
        {
        }

        public async Task RunTasksOnEachRequestAsync()
        {
            foreach (IRunOnEachRequest task in RunOnEachRequest)
            {
                 await task.ExecuteAsync();
            }
        }

        public async Task RunTasksAfterEachRequestAsync()
        {
            foreach (IRunAfterEachRequest task in RunAfterEachRequest)
            {
                await task.ExecuteAsync();
            }
        }

        public async Task RunTasksOnErrorAsync()
        {
            foreach (IRunOnError task in RunOnError)
            {
                await task.ExecuteAsync();
            }
        }
    }
}
