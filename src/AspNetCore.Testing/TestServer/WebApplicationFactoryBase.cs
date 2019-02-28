using AspNetCore.Base.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Testing.TestServer
{
    //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2
    public abstract class WebApplicationFactoryBase<TEntryPoint>
    : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private string _environment;

        public static readonly string AntiForgeryFieldName = "__AFTField";
        public static readonly string AntiForgeryCookieName = "AFTCookie";

        public WebApplicationFactoryBase(string environment = "Integration")
        {
            _environment = environment;
        }

        public IFeatureCollection ServerFeatures
        {
            get { return Server.Host.ServerFeatures; }
        }

        public IServiceProvider Services
        {
            get { return Server.Host.Services; }
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder();
            //The default implementation of this method looks for a public static IWebHostBuilder
            //CreateDefaultBuilder(string[] args) method defined on the entry point of the
            //assembly of TEntryPoint and invokes it passing an empty string array as arguments.
            //Sets Environment to Development
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment(_environment)
             .ConfigureServices(services =>
             {
                 ConfigureServices(services);
             })
            .ConfigureTestServices(services =>
            {
                ConfigureTestServices(services);
            });
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.CookieName = AntiForgeryCookieName;
                options.FormFieldName = AntiForgeryFieldName;
            });
        }

        //Inject mock services
        public abstract void ConfigureTestServices(IServiceCollection services);

        protected override Microsoft.AspNetCore.TestHost.TestServer CreateServer(IWebHostBuilder builder)
        {
            //Set ContentRoot here
            //builder.UseContentRoot("");
            //UseSolutionRelativeContentRoot(builder, typeof(TEntryPoint).Assembly.GetName().Name);
            //relies on WebApp being located in Solution\WebApp and not Solution\src\WebApp


            //new TestServer(builder)
            var server =  base.CreateServer(builder);
            InitializeWebHost(server.Host);
            return server;
        }

        public virtual string GetContentRoot()
        {
            var contentRoot = GetContentRootFromConfig();
            if (contentRoot != null)
                return contentRoot;

            //Add reference to Microsoft.AspNetCore.Mvc.Testing
            contentRoot = GetContentRootFromAttribute();
            if (contentRoot != null && Directory.Exists(contentRoot))
                return contentRoot;

            contentRoot = GetSolutionRelativeContentRoot(typeof(TEntryPoint).Assembly.GetName().Name);
            if (contentRoot != null && Directory.Exists(contentRoot))
                return contentRoot;

            contentRoot = GetSolutionRelativeContentRoot($@"src\{typeof(TEntryPoint).Assembly.GetName().Name}");
            if (contentRoot != null && Directory.Exists(contentRoot))
                return contentRoot;

            return null;
        }

        public string GetBinRelativeContentRoot(string binRelativePath)
        {
            var testProjectPath = AppContext.BaseDirectory;
            var contentRoot = Path.GetFullPath(Path.Combine(testProjectPath, binRelativePath));
            return contentRoot;
        }

        public string GetContentRootFromConfig()
        {
            var config = new ConfigurationBuilder()
                  .AddEnvironmentVariables("ASPNETCORE_");

            var configEnvironment = config.Build();

            var assemblyName = typeof(TEntryPoint).Assembly.GetName().Name;
            var settingSuffix = assemblyName.ToUpperInvariant().Replace(".", "_");
            var settingName = $"TEST_CONTENTROOT_{settingSuffix}";

            return configEnvironment[settingName];
        }

        public string GetContentRootFromAttribute()
        {
            var metadataAttributes = GetContentRootMetadataAttributes();

            string contentRoot = null;
            for (var i = 0; i < metadataAttributes.Length; i++)
            {
                var contentRootAttribute = metadataAttributes[i];
                var contentRootCandidate = Path.Combine(
                    AppContext.BaseDirectory,
                    contentRootAttribute.ContentRootPath);

                var contentRootMarker = Path.Combine(
                    contentRootCandidate,
                    Path.GetFileName(contentRootAttribute.ContentRootTest));

                if (File.Exists(contentRootMarker))
                {
                    contentRoot = contentRootCandidate;
                    break;
                }
            }

            return contentRoot;
        }

        private WebApplicationFactoryContentRootAttribute[] GetContentRootMetadataAttributes()
        {
            return GetContentRootMetadataAttributes(
                 typeof(TEntryPoint).Assembly.FullName,
                typeof(TEntryPoint).Assembly.GetName().Name);
        }

        private WebApplicationFactoryContentRootAttribute[] GetContentRootMetadataAttributes(
            string tEntryPointAssemblyFullName,
            string tEntryPointAssemblyName)
        {
            var testAssembly = GetTestAssemblies();
            var metadataAttributes = testAssembly
                .SelectMany(a => a.GetCustomAttributes(typeof(WebApplicationFactoryContentRootAttribute), true).Select(at => (WebApplicationFactoryContentRootAttribute)at))
                .Where(a => string.Equals(a.Key, tEntryPointAssemblyFullName, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(a.Key, tEntryPointAssemblyName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Priority)
                .ToArray();

            return metadataAttributes;
        }

        public string GetSolutionRelativeContentRoot(
           string solutionRelativePath,
           string solutionName = "*.sln")
        {
            return GetSolutionRelativeContentRoot(solutionRelativePath, AppContext.BaseDirectory, solutionName);
        }

        public string GetSolutionRelativeContentRoot(
            string solutionRelativePath,
            string applicationBasePath,
            string solutionName = "*.sln")
        {
            if (solutionRelativePath == null)
            {
                throw new ArgumentNullException(nameof(solutionRelativePath));
            }

            if (applicationBasePath == null)
            {
                throw new ArgumentNullException(nameof(applicationBasePath));
            }

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionPath = Directory.EnumerateFiles(directoryInfo.FullName, solutionName).FirstOrDefault();
                if (solutionPath != null)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            return null;
        }

        public virtual void InitializeWebHost(IWebHost host)
        {
            host.InitAsync().GetAwaiter().GetResult();
        }

        public HttpClient CreateClientToTestSecureEndpoint()
        {
            return CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        public async Task<(string fieldValue, string cookieValue)> ExtractAntiForgeryValues(HttpResponseMessage response)
        {
            return (ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync()),
                                            ExtractAntiForgeryCookieValueFrom(response));
        }

        private string ExtractAntiForgeryCookieValueFrom(HttpResponseMessage response)
        {
            string antiForgeryCookie =
                        response.Headers
                                .GetValues("Set-Cookie")
                                .FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

            if (antiForgeryCookie is null)
            {
                throw new ArgumentException(
                    $"Cookie '{AntiForgeryCookieName}' not found in HTTP response",
                    nameof(response));
            }

            string antiForgeryCookieValue =
                SetCookieHeaderValue.Parse(antiForgeryCookie).Value.ToString();

            return antiForgeryCookieValue;
        }

        private string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in HTML", nameof(htmlBody));
        }

    }
}
