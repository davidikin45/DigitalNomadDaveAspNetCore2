using AspNetCore.Base.ElasticSearch;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using System.Reflection;

namespace AspnetCore.Base
{
    //http://mygeekjourney.com/asp-net-core/integrating-serilog-asp-net-core/
    //https://www.carlrippon.com/asp-net-core-logging-with-serilog-and-sql-server/
    //Logging
    //Trace = 0
    //Debug = 1 -- Developement Standard
    //Information = 2 -- LogFactory Default
    //Warning = 3 -- Production Standard
    //Error = 4
    //Critical = 5

    public class Logging
    {
        //https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker
        public static void Init(IConfiguration configuration, string elasticUri = null)
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var loggerConfiguration = new LoggerConfiguration()
             .ReadFrom.Configuration(configuration)
             .Enrich.FromLogContext()
             .Enrich.WithExceptionDetails() //Include exception.data
             .Enrich.WithMachineName()
             .Enrich.WithProperty("Assembly", $"{name.Name}")
             .Enrich.WithProperty("Version", $"{name.Version}")
             .AddElasticSearchLogging(configuration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
