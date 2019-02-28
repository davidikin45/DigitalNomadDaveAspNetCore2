using AspNetCore.Testing;
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
        public async System.Threading.Tasks.Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
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
    }
}
