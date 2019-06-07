using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureWebHost(this IWebHostBuilder webHostBuilder, Action<IWebHostBuilder> configure)
        {
            configure(webHostBuilder);

            return webHostBuilder;
        }
    }
}
