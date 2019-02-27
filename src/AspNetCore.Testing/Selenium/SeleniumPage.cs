using OpenQA.Selenium;
using System;

namespace AspNetCore.Testing.Selenium
{
    public class SeleniumPage
    {
        public IWebDriver Driver { get; }

        private string _pagePath = "";

        public SeleniumPage(IWebDriver driver, string pagePath)
        {
            _pagePath = pagePath;
            Driver = driver;
        }

        public string Path { get
            {
                return new Uri(Driver.Url).GetLeftPart(UriPartial.Path).Replace(new Uri(Driver.Url).GetLeftPart(UriPartial.Authority)+"/","");
            }
        }
    
        public string Html
        {
            get
            {
                return Driver.FindElement(By.XPath("//*")).GetAttribute("outerHTML");
            }
        }

        public void NavigateTo()
        {
            var root = new Uri(Driver.Url).GetLeftPart(UriPartial.Authority);

            var url = $"{root}/{_pagePath}";

            Driver.Navigate().GoToUrl(url);
        }

        public IWebElement FirstError
        {
            get { return Driver.FindElement(By.CssSelector(".validation-summary-errors ul > li")); }
        }

        public string FirstErrorMessage => FirstError.Text;


        public void EnterFormValue(string id, string value)
        {
            Driver.FindElement(By.Id(id)).SendKeys(value);
        }

        public SeleniumPage ClickLink(string text, string cssClass)
        {
            Driver.FindElement(By.XPath($@"//a[contains(text(), ""{text}"") and @class=""{cssClass}""]")).Click();

            return new SeleniumPage(Driver, Path);
        }

        public SeleniumPage Submit(string id = "Submit")
        {
            Driver.FindElement(By.Id(id)).Click();

            return new SeleniumPage(Driver, _pagePath);
        }
    }
}
