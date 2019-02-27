namespace AspNetCore.Base.Settings
{
    public class PasswordSettings
    {
        public int MaxFailedAccessAttemptsBeforeLockout { get; set; }
        public int LockoutMinutes { get; set; }
        public string PasswordResetCallbackUrl { get; set; }
        public bool RequireDigit { get; set; }
        public int RequiredLength { get; set; }
        public int RequiredUniqueChars { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
    }
}
