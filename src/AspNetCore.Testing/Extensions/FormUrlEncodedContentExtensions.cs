using AspNetCore.Testing.Helpers;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.Testing.Extensions
{
    public static class FormUrlEncodedContentExtensions
    {
        public static FormUrlEncodedContent FromObject(object value)
        {
            return new FormUrlEncodedContent(QueryStringHelper.ToKeyValue(value) ?? new Dictionary<string, string>());
        }
    }
}
