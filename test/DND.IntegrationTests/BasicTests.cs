using AspNetCore.Base;
using AspNetCore.Testing;
using AspNetCore.Testing.TestServer;
using DND.Web.Areas.Frontend.Controllers.Home.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTests
{
    public class BasicTests : IClassFixture<EnvironmentVariablesFixture>, IClassFixture<WebAppFactory>
    {
        private readonly WebAppFactory _factory;

        public BasicTests(WebAppFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/en")]
        [InlineData("/en/blog")]
        [InlineData("/en/gallery")]
        [InlineData("/en/videos")]
        [InlineData("/en/bucket-list")]
        [InlineData("/en/travel-map")]
        [InlineData("/en/about")]
        [InlineData("/en/work-with-me")]
        [InlineData("/en/contact")]
        public async Task RenderPageSuccessfully(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetAllPublicRoutesAndRenderPagesSuccessfully()
        {
            // Arrange
            var client = _factory.CreateClient();

            var routeInfo = _factory.GetAllRoutes();

            var testRoutes = new List<string>();
            foreach (var action in routeInfo.Actions)
            {
                string path = action.Path;

                IEnumerable<string> httpMethods = action.HttpMethods;
                bool authorized = action.Authorized;

                if (!authorized && path != null && (httpMethods == null || httpMethods.ToList().Contains(HttpMethod.Get.Method)))
                {
                    testRoutes.Add(path);
                }
            }

            foreach (var route in testRoutes)
            {
                var testRoute = route.Replace("{culture:cultureCheck}", "en");
                if (!testRoute.Contains("{") && !testRoute.Contains("api"))
                {
                    var response = await client.GetAsync(testRoute);
                    Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
                }
            }

            Assert.True(true);
        }

        [Fact]
        public async Task AcceptContactFormPost()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Get initial response that contains anti forgery tokens
            HttpResponseMessage initialResponse = await client.GetAsync("/en/contact");
            var antiForgeryValues = await _factory.ExtractAntiForgeryValues(initialResponse);

            // Create POST request, adding anti forgery cookie and form field
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "/en/contact");

            postRequest.Headers.Add("Cookie", $"{TestServerFixtureBase<object>.AntiForgeryCookieName}={antiForgeryValues.cookieValue}");

            var formData = new Dictionary<string, string>
            {
                {TestServerFixtureBase<object>.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {nameof(ContactViewModel.Name),"James Smith"},
                {nameof(ContactViewModel.Email),"test@gmail.com"},
                {nameof(ContactViewModel.Website),""},
                {nameof(ContactViewModel.Subject),"Enquiry"},
                {nameof(ContactViewModel.Message),"This is a test message"}
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            HttpResponseMessage postResponse = await client.SendAsync(postRequest);

            postResponse.EnsureSuccessStatusCode();

            var responseString = await postResponse.Content.ReadAsStringAsync();

            Assert.Contains(Messages.MessageSentSuccessfully, responseString);
        }
    }
}
