namespace AspNetCore.Base.Settings
{
    public class SwitchSettings
    {
        public bool EnableHsts { get; set; }
        public bool EnableRedirectHttpToHttps { get; set; }
        public bool EnableRedirectNonWwwToWww { get; set; }
        public bool EnableMVCModelValidation { get; set; }
        public bool EnableHelloWorld { get; set; }
        public bool EnableSwagger { get; set; }
        public bool EnableResponseCompression { get; set; }
        public bool EnableIpRateLimiting { get; set; }
        public bool EnableIFramesGlobal { get; set; }
        public bool EnableCors { get; set; }
        public bool EnableSignalR { get; set; }
        public bool EnableCookieConsent { get; set; }
        public bool EnableResponseCaching { get; set; }
        public bool EnableETags { get; set; }
        public bool EnableHangfire { get; set; }
        public bool EnableMultiTenancy { get; set; }
    }
}
