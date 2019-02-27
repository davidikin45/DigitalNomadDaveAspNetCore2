using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCore.Base.ActionResults
{
    public class HTMLActionResult : IActionResult
    {
        public string Html { get; private set; }

        public HTMLActionResult(string html)
        {
            Html = html;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(Html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }
    }
}
