using AspNetCore.Testing.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Testing.TestServer
{
    public abstract class TestServerFixtureBase<T> : IDisposable where T: class
    {
        private Microsoft.AspNetCore.TestHost.TestServer _testServer;
        private string _webAppRelativePath;
        private string _environment;
        private string _netVersion;
        private Func<string, string, IConfiguration> _configBuilder;

        public HttpClient Client { get; private set; }

        public static readonly string AntiForgeryFieldName = "__AFTField";
        public static readonly string AntiForgeryCookieName = "AFTCookie";

        public TestServerFixtureBase(string webAppRelativePath, string environment, string netVersion, Func<string, string, IConfiguration> configBuilder)
        {
            _webAppRelativePath = webAppRelativePath;
            _environment = environment;
            _netVersion = netVersion;
            _configBuilder = configBuilder;
            // Do "global" initialization here; Only called once.
        }

        public IFeatureCollection ServerFeatures
        {
            get { return _testServer.Host.ServerFeatures; }
        }

        public IServiceProvider Services
        {
            get { return _testServer.Host.Services;  }
        }

        public void Launch()
        {
            var builder = new WebHostBuilder();

            builder.UseContentRoot(GetContentRootPath())
           .UseEnvironment(_environment)
           .ConfigureLogging(factory =>
           {
               factory.AddConsole();
           })
           .UseAutofac()
           .UseConfiguration(_configBuilder.Invoke(_environment, builder.GetSetting(WebHostDefaults.ContentRootKey)))
           .UseStartup<T>()
           .ConfigureServices(services =>
           {

               services.AddAntiforgery(options => {
                   options.CookieName = AntiForgeryCookieName;
                   options.FormFieldName = AntiForgeryFieldName;
               });

               //Test Server Fix
               //https://github.com/aspnet/Hosting/issues/954
               //https://github.com/Microsoft/vstest/issues/428
               var assembly = typeof(T).GetTypeInfo().Assembly;
               services.ConfigureRazorViewEngineForTestServer(assembly, _netVersion);
           });

            _testServer = new Microsoft.AspNetCore.TestHost.TestServer(builder);

            Client = _testServer.CreateClient();
        }

        public async Task<(string fieldValue, string cookieValue)> ExtractAntiForgeryValues(HttpResponseMessage response)
        {
            return (ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync()),
                                            ExtractAntiForgeryCookieValueFrom(response));
        }

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            var contentPath = Path.GetFullPath(Path.Combine(testProjectPath, _webAppRelativePath));
            return contentPath;
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

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
