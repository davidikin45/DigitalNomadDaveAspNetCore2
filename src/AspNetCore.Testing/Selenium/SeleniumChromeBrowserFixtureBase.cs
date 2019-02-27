using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace AspNetCore.Testing.Selenium
{
    public abstract class SeleniumChromeBrowserFixtureBase : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        private string _url;
        private bool _hideBrowser;

        public SeleniumChromeBrowserFixtureBase(string url, bool hideBrowser)
        {
            _url = url;
            _hideBrowser = hideBrowser;

            LaunchBrowser();
        }

        private void LaunchBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            //Hide browser
            if (_hideBrowser)
            {
                options.AddArguments("--headless");
            }

            Driver = new ChromeDriver(options);
            Driver.Navigate().GoToUrl(_url);
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
