namespace AspNetCore.Base.Settings
{
    public class UserSettings
    {
        public bool RequireConfirmedEmail { get; set; }
        public bool RequireUniqueEmail { get; set; }
        public int RegistrationEmailConfirmationExprireDays { get; set; }
        public int ForgotPasswordEmailConfirmationExpireHours { get; set; }
        public int UserDetailsChangeLogoutMinutes { get; set; }
    }
}
