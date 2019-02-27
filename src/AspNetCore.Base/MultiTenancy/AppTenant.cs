using AspNetCore.Base.Data.Converters;
using AspNetCore.Base.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Base.MultiTenancy
{
    public class AppTenant : EntityBase<string>, IEntitySoftDelete
    {
        public string Name { get; set; }

        [Json]
        public string[] RequestIpAddresses { get; set; }
        [Json]
        public string[] HostNames { get; set; }
        [Json]
        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();

        public string GetConnectionString(string name)
        {
            if(ConnectionStrings.ContainsKey(name))
            {
                return ConnectionStrings[name];
            }

            return null;
        }

        public bool IpAddressAllowed(string ip)
        {
            return 
                RequestIpAddresses == null 
                || RequestIpAddresses.Length == 0 
                || RequestIpAddresses.Where(i => !i.Contains("*") || i.EndsWith("*")).Any(i => ip.StartsWith(i.Replace("*", "")))
                || RequestIpAddresses.Where(i => i.StartsWith("*")).Any(i => ip.EndsWith(i.Replace("*", "")));
        }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    }
}
