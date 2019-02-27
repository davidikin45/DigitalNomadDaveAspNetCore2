namespace AspNetCore.Base.Settings
{
    public class AuthenticationSettings
    {
        public bool AllowAnonymousUsers { get; set; }
        public Authentication Application { get; set; }
        public Authentication JwtToken { get; set; }
        public Authentication OpenIdConnect { get; set; }
        public Authentication OpenIdConnectJwtToken { get; set; }
        public Authentication Google { get; set; }
        public Authentication Facebook { get; set; }
    }

    public class Authentication
    {
        public bool Enable { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
