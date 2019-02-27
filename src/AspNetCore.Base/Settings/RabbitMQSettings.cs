namespace AspNetCore.Base.Settings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SubscriptionClientName { get; set; }
        public int RetryCount { get; set; } = 5;
    }
}
