using AspNetCore.Testing.Processes;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;

namespace AspNetCore.Testing.DotNetRun
{
    public abstract class DotNetRunFixtureBase : IDisposable
    {
        private SimpleProcess _process;
        private string _environment;
        private string _webAppRelativePath;

        private string _url;
        private bool _hideBrowser;

        public DotNetRunFixtureBase(string webAppRelativePath, string environment, string url, bool hideBrowser)
        {
            _webAppRelativePath = webAppRelativePath;
            _environment = environment;
            _hideBrowser = hideBrowser;
            _url = url;

            string filename = "dotnet.exe";
            string args = $@"run --project {GetContentRootPath()}\ environment={_environment} --no-build --urls ""{_url}""";

            _process = new SimpleProcess(filename, args, _hideBrowser);
        }

        public void Launch()
        {
            if (!string.IsNullOrEmpty(_webAppRelativePath) && !string.IsNullOrEmpty(_environment) && _url.Contains("localhost"))
            {
                _process.StartProcess();
            }
        }

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            var contentPath = Path.GetFullPath(Path.Combine(testProjectPath, _webAppRelativePath));
            return contentPath;
        }

        public bool IsRunning
        {
            get
            {
                return _process.IsRunning;
            }
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}
