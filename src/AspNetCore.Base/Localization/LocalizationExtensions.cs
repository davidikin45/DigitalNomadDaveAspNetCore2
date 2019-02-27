using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Base.Localization
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddRequestLocalizationOptions(this IServiceCollection services, string defaultCulture, bool supportAllCountryFormatting, bool supportAllLanguagesFormatting, bool supportUICultureFormatting, bool allowDefaultCultureLanguage, params string[] supportedUICultures)
        {
            //https://andrewlock.net/adding-localisation-to-an-asp-net-core-application/
            //Default culture should be set to where the majority of traffic comes from.
            //If the client sends through "en" and the default culture is "en-AU". Instead of falling back to "en" it will fall back to "en-AU".
            var defaultLanguage = defaultCulture.Split('-')[0];

            //Support all formats for numbers, dates, etc.
            var formatCulturesList = new List<string>() { };

            if (supportAllLanguagesFormatting)
            {
                //Languages = en
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    if (!formatCulturesList.Contains(ci.TwoLetterISOLanguageName) && (allowDefaultCultureLanguage || ci.TwoLetterISOLanguageName != defaultLanguage))
                    {
                        formatCulturesList.Add(ci.TwoLetterISOLanguageName);
                    }

                    if (supportAllCountryFormatting)
                    {
                        foreach (CultureInfo co in CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(co => co.TwoLetterISOLanguageName == ci.TwoLetterISOLanguageName))
                        {
                            if (!formatCulturesList.Contains(co.Name))
                            {
                                formatCulturesList.Add(co.Name);
                            }
                        }
                    }
                }
            }

            //Countries = en-US
            if (supportAllCountryFormatting && !supportAllLanguagesFormatting)
            {
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    if (!formatCulturesList.Contains(ci.Name))
                    {
                        formatCulturesList.Add(ci.Name);
                    }
                }
            }

            if (supportUICultureFormatting)
            {
                foreach (var supportedUICulture in supportedUICultures)
                {
                    if (supportedUICulture.Length == 2)
                    {
                        var neutralCulture = CultureInfo.GetCultureInfo(supportedUICulture);

                        if (!formatCulturesList.Contains(neutralCulture.TwoLetterISOLanguageName) && (allowDefaultCultureLanguage || neutralCulture.TwoLetterISOLanguageName != defaultLanguage))
                        {
                            formatCulturesList.Add(neutralCulture.TwoLetterISOLanguageName);
                        }

                        foreach (CultureInfo co in CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(co => co.TwoLetterISOLanguageName == neutralCulture.TwoLetterISOLanguageName))
                        {
                            if (!formatCulturesList.Contains(co.Name))
                            {
                                formatCulturesList.Add(co.Name);
                            }
                        }
                    }
                    else
                    {
                        if (!formatCulturesList.Contains(supportedUICulture))
                        {
                            formatCulturesList.Add(supportedUICulture);
                        }
                    }
                }
            }

            var supportedFormatCultures = formatCulturesList.Select(x => new CultureInfo(x)).ToArray();

            var supportedUICultureInfoList = new List<CultureInfo>() { };

            foreach (var supportedUICulture in supportedUICultures)
            {
                supportedUICultureInfoList.Add(new CultureInfo(supportedUICulture));
            }

            var defaultUICulture = supportedUICultures[0];

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: defaultCulture, uiCulture: defaultUICulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedFormatCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedUICultureInfoList
            };

            //Default culture providers
            //1. Query string
            //2. Cookie
            //3. Accept-Language header

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.2
            //https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/
            //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
            //https://gunnarpeipman.com/aspnet/aspnet-core-simple-localization/
            //http://sikorsky.pro/en/blog/aspnet-core-culture-route-parameter

            //Route("{culture}/{ui-culture}/[controller]")]
            //[Route("{culture}/[controller]")]

            var routeDataRequestProvider = new RouteDataRequestCultureProvider() { Options = options, RouteDataStringKey = "culture", UIRouteDataStringKey = "ui-culture" };

            //options.RequestCultureProviders.Insert(0, routeDataRequestProvider);

            options.RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                 routeDataRequestProvider,
                 new QueryStringRequestCultureProvider() { QueryStringKey = "culture", UIQueryStringKey = "ui-culture" },
                 new CookieRequestCultureProvider(),
                 new AcceptLanguageHeaderRequestCultureProvider(),
            };

            services.AddSingleton(options);

            return services;
        }

        public static MvcOptions AddCultureRouteConvention(this MvcOptions options)
        {
            options.Conventions.Insert(0, new LocalizationConvention());

            return options;
        }

        public static MvcOptions AddOptionalCultureRouteConvention(this MvcOptions options)
        {
            options.Conventions.Insert(0, new LocalizationConvention(true));

            return options;
        }

        public static IRouteBuilder RedirectCulturelessToDefaultCulture(this IRouteBuilder routes, RequestLocalizationOptions localizationOptions)
        {
            routes.MapRoute("{culture:cultureCheck}/{*path}", ctx => {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            });

            routes.MapRoute("{*path}", (RequestDelegate)(ctx =>
            {
                var defaultCulture = localizationOptions.DefaultRequestCulture.Culture.Name;

                var cultureFeature = ctx.Features.Get<IRequestCultureFeature>();
                var actualCulture = cultureFeature?.RequestCulture.Culture.Name;
                var actualCultureLanguage = cultureFeature?.RequestCulture.Culture.TwoLetterISOLanguageName;

                var path = ctx.GetRouteValue("path") ?? string.Empty;
                var culturedPath = $"{ctx.Request.PathBase}/{actualCulture}/{path}{ctx.Request.QueryString.ToString()}";
                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));

            return routes;
        }
    }
}
