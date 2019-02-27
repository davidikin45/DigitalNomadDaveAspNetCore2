using AspNetCore.Base.Email;
using AspNetCore.Base.MvcExtensions;
using AspNetCore.Base.MvcServices;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AspNetCore.Base.Controllers.Mvc
{
    public abstract class MvcControllerHomeBase : MvcControllerBase
    {
        public MvcControllerHomeBase()
        {

        }

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly NavigationService _navigationService;

        public MvcControllerHomeBase(IMapper mapper, IEmailService emailService, AppSettings appSettings, IHostingEnvironment hostingEnvironment, NavigationService navigationService)
            : base(mapper, emailService, appSettings)
        {
            _hostingEnvironment = hostingEnvironment;
            _navigationService = navigationService;
        }
 
        protected abstract string SiteTitle { get; }
        protected abstract string SiteDescription { get; }
        protected abstract string SiteUrl { get; }

        [HttpPost("set-language-cookie")]
        public IActionResult SetLanguageCookie(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost("set-language-url")]
        public IActionResult SetLanguageUrl(string culture, string returnUrl)
        {
            var culturedPath = $"{Request.PathBase}/{culture}{returnUrl}";

            return LocalRedirect($"~{culturedPath}");
        }

        [HttpGet("culture")]
        public string GetCulture()
        {
            return $"CurrentCulture:{CultureInfo.CurrentCulture.Name}, CurrentUICulture:{CultureInfo.CurrentUICulture.Name}";
        }

        //0.8-1.0: Homepage, subdomains, product info, major features
        //0.4-0.7: Articles and blog entries, category pages, FAQs
        protected virtual Task<IList<SitemapNode>> GetSitemapNodes(CancellationToken cancellationToken)
        {
            var siteUrl = AppSettings.SiteUrl;

            List<SitemapNode> nodes = new List<SitemapNode>();

            nodes.Add(
                new SitemapNode()
                {
                    Url = siteUrl,
                    Priority = 1
                });

            foreach (dynamic menuItem in _navigationService.Menu)
            {
                nodes.Add(
                  new SitemapNode()
                  {
                      Url = Url.AbsoluteUrl((string)menuItem.Action, (string)menuItem.Controller, AppSettings, null),
                      Priority = 0.9
                  });
            }

            IList<SitemapNode> result = nodes;

            return Task.FromResult(result);
        }

        protected string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        protected class SitemapNode
        {
            public SitemapFrequency? Frequency { get; set; }
            public DateTime? LastModified { get; set; }
            public double? Priority { get; set; }
            public string Url { get; set; }
        }

        protected enum SitemapFrequency
        {
            Never,
            Yearly,
            Monthly,
            Weekly,
            Daily,
            Hourly,
            Always
        }
    }
}
