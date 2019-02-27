using AspNetCore.Base.Extensions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace AspNetCore.Base.MvcServices
{
    public class BundleConfigService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly object _config;

        public BundleConfigService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = JsonConvert.DeserializeObject(File.ReadAllText(_hostingEnvironment.MapContentPath("bundleconfig.json")));
        }

        public dynamic Config
        {
            get { return _config; }
        }

        public dynamic Bundle(string outputFileName)
        {
            foreach (var bundle in Config)
            {
                if ((string)bundle.outputFileName == outputFileName)
                {
                    return bundle;
                }
            }
            return null;
        }
    }
}
