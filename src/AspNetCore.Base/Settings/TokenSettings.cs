namespace AspNetCore.Base.Settings
{
    public class TokenSettings
    {
        public int ExpiryMinutes { get; set; }
        public string Key { get; set; }
        public string PublicCertificatePath { get; set; }
        public string PrivateCertificatePath { get; set; }
        public string PrivateCertificatePasword { get; set; }
        public string PublicKeyPath { get; set; }
        public string PrivateKeyPath { get; set; }
        public string LocalIssuer { get; set; }
        public string ExternalIssuers { get; set; }
        public string Audiences { get; set; }
    }
}
