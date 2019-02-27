using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Settings
{
    public class LocalizationSettings
    {
        public string DefaultCulture { get; set; }
        public string[] SupportedUICultures { get; set; }

        public bool SupportAllLanguagesFormatting { get; set; }
        public bool SupportAllCountriesFormatting { get; set; }
        public bool SupportUICultureFormatting { get; set; }
        public bool SupportDefaultCultureLanguageFormatting { get; set; }

        public bool AlwaysIncludeCultureInUrl { get; set; }
    }
}
