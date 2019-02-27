using System;

namespace AspNetCore.Base.HostedServices
{
    public class CronJobAttribute : Attribute
    {
        public string[] Schedules { get; }
        public CronJobAttribute(params string[] schedules)
        {
            Schedules = schedules;
        }
    }
}
