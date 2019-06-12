using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace AspNetCore.Base.Extensions
{
    public static class RequestHelper
    {
        private static readonly Comparison<MediaTypeSegmentWithQuality> _sortFunction = (left, right) =>
        {
            return left.Quality > right.Quality ? -1 : (left.Quality == right.Quality ? 0 : 1);
        };

        public static bool IsApi(this HttpRequest request)
        {
            return !IsMvc(request);
        }

        public static bool IsMvc(this HttpRequest request)
        {
            if (request.Path.ToString().Contains("/api"))
                return false;

            var result = new List<MediaTypeSegmentWithQuality>();
            
            AcceptHeaderParser.ParseAcceptHeader(request.Headers[HeaderNames.Accept], result);
            for (var i = 0; i < result.Count; i++)
            {
                if(result[i].MediaType == "text/html")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
