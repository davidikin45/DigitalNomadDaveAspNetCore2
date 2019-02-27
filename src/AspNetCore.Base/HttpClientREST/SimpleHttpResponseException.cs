using System;
using System.Net;

namespace AspNetCore.Base.HttpClientREST
{
    public class SimpleHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }

        public SimpleHttpResponseException(HttpStatusCode statusCode, string reasonPhrase, string content) : base(content)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }
    }
}
