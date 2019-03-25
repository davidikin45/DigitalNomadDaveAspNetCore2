using AspnetCore.Base;
using AspNetCore.Base.Azure;
using AspNetCore.Base.DependencyInjection;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Hosting;
using AspNetCore.Base.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AspNetCore.Base
{
    public abstract class ProgramSingleTenantBase<TStartup> where TStartup : class
    {
        public static IConfiguration Configuration;

        public static IConfiguration BuildWebHostConfiguration(string environment, string contentRoot)
        {
            return Config.Build(new string[] { $"environment={environment}" }, contentRoot, typeof(TStartup).Assembly.GetName().Name);
        }

        public static async Task<int> RunApp(string[] args)
        {
            Configuration = Config.Build(args, Directory.GetCurrentDirectory(), typeof(TStartup).Assembly.GetName().Name);

            Logging.Init(Configuration);

            try
            {
                Log.Information("Getting the motors running...");

                var host = CreateWebHostBuilder(args).Build();

                //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/
                //Even though the tasks run after the IConfiguration and DI container configuration has completed, they run before the IStartupFilters have run and the middleware pipeline has been configured.
                //await host.InitAsync();

                //AppStartup.Configure will be called here
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                // These two settings allow an error page to be shown rather than throwing exception on startup
                // Need to be careful putting code after IWebHostBuilder.Build()
                .CaptureStartupErrors(true)
                //.UseSetting("detailedErrors", "true") // Better to put this in appsettings
                .ConfigureKestrel((context, options) =>
                {
                    if(context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsIntegration())
                    {
                        options.ListenAnyIP(5000);
                        options.ListenAnyIP(5001, listenOptions => {
                            //listenOptions.UseHttps(new X509Certificate2("certificates\\localhost.private.pfx", "password"));
                            listenOptions.UseHttps();
                        });
                    }

                    options.AllowSynchronousIO = true;
                    options.AddServerHeader = false;
                }
                )
                .UseAutofac()
                .UseConfiguration(Configuration) ////IWebHostBuilder configuration is added to the app's configuration, but the converse isn't true. ConfigureAppConfiguration doesn't affect the IWebHostBuilder configuration.
                .UseAzureKeyVault()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                })
                .ConfigureServices(services => {

                })
                .UseSerilog()
                .UseTaskExecutingServer()
                .UseStartup<TStartup>();

        //WebHostBuilder - https://github.com/aspnet/Hosting/blob/3483a3250535da6f291326f3f5f1e3f66ca09901/src/Microsoft.AspNetCore.Hosting/WebHostBuilder.cs
        //WebHost.CreateDefaultBuilder(args) - https://github.com/aspnet/MetaPackages/blob/release/2.1/src/Microsoft.AspNetCore/WebHost.cs
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.1

        // Only used by EF Core Tooling if IDesignTimeDbContextFactory is not implemented
        // Generally its not good practice to DB in the MVC Project so best to use IDesignTimeDbContextFactory
        //https://wildermuth.com/2017/07/06/Program-cs-in-ASP-NET-Core-2-0
        // public static IWebHost BuildWebHost(string[] args)
        //{
        // Configuration = BuildWebHostConfiguration(args, Directory.GetCurrentDirectory());
        //return CreateWebHostBuilder(args).Build();
        //}
    }
}
