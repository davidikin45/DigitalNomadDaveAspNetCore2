using AspNetCore.Base.Extensions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace AspNetCore.Base.MvcServices
{
    public class NavigationService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly object _menu;
        private readonly object _adminMenu;

        public NavigationService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _menu = JsonConvert.DeserializeObject(File.ReadAllText(_hostingEnvironment.MapContentPath("navigation.json")));
            _adminMenu = JsonConvert.DeserializeObject(File.ReadAllText(_hostingEnvironment.MapContentPath("navigation-admin.json")));
        }

        public dynamic Menu
        {
            get { return _menu; }
        }

        public dynamic AdminMenu
        {
            get { return _adminMenu; }
        }
    }
}
