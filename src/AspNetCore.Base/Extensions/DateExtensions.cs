using AspNetCore.Base.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace AspNetCore.Base.Extensions
{
    public static class DateExtensions
    {
        public static string ToISO8601(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("s") + "Z";
        }

        public static string ToDaysTil(this DateTime value, DateTime endDateTime)
        {
            var ts = new TimeSpan(endDateTime.Ticks - value.Ticks);
            var delta = ts.TotalSeconds;
            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds";
            }
            if (delta < 120)
            {
                return "a minute";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month" : months + " months";
            }
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year" : years + " years";
        }

        public static DateTime ToStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime ToEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        //Json.NET returns all dates as Unspecified

        //UTC > LocalTime = Fine
        //LocalTime > UTC = Fine

        //Unspecified(UTC) > LocalTime
        //Unspecified(LocalTime) > UTC

        public static DateTime ToConfigLocalTime(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ);
        }

        public static string ToConfigLocalTimeString(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return String.Format("{0} ({1})", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString(), appSettings.TimezoneAbbr);
        }

        public static string ToConfigLocalTimeStringNoTimezone(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return String.Format("{0}", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString());
        }

        public static DateTime FromConfigLocalTimeToUTC(this IHtmlHelper htmlhelper, DateTime localConfigDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return TimeZoneInfo.ConvertTimeToUtc(localConfigDT, istTZ);
        }
    }
}
