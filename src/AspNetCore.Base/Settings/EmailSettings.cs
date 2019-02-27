namespace AspNetCore.Base.Settings
{
    public class EmailSettings
    {
        public string ToDisplayName { get; set; }
        public string FromDisplayName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        public bool SendEmailsViaSmtp { get; set; }
        public bool WriteEmailsToFileSystem { get; set; }
        public string FileSystemFolder { get; set; }
    }
}
