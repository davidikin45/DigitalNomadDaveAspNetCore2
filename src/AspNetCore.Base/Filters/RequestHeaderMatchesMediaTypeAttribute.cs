using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Linq;

namespace AspNetCore.Base.Filters
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string[] _mediaTypes;
        private readonly string _requestHeaderToMatch;

        public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch,
            string[] mediaTypes)
        {
            _requestHeaderToMatch = requestHeaderToMatch;
            _mediaTypes = mediaTypes;
        }

        public int Order
        {
            get { return 0; }
        }

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            if (!requestHeaders.ContainsKey(_requestHeaderToMatch))
            {
                return false;
            }

            // if one of the media types matches, return true
            foreach (var mediaType in _mediaTypes)
            {
                var headerValues = requestHeaders[_requestHeaderToMatch]
                    .ToString().Split(',').ToList();
                foreach (var headerValue in headerValues)
                {
                    if (string.Equals(headerValue, mediaType,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
    }
}
