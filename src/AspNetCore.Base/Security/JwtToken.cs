using System;

namespace AspNetCore.Base.Security
{
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
