using AspNetCore.Base.Tasks;
using Autofac;

namespace AspNetCore.Base.Hosting
{
    public static class TasksAutofacExtensions
    {
        public static void RegisterTaskRunners(this ContainerBuilder builder)
        {
            builder.RegisterType<TaskRunnerAfterApplicationConfiguration>().AsSelf().PropertiesAutowired();
            builder.RegisterType<TaskRunnerDbInitialization>().AsSelf().PropertiesAutowired();
            builder.RegisterType<TaskRunnerInitialization>().AsSelf().PropertiesAutowired();
            builder.RegisterType<TaskRunnerRequests>().AsSelf().PropertiesAutowired();
        }
    }
}
