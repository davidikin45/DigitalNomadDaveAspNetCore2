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
        [InlineData("/")]
        [InlineData("/blog")]
        [InlineData("/gallery")]
        [InlineData("/videos")]
        [InlineData("/bucket-list")]
        [InlineData("/travel-map")]
        [InlineData("/about")]
        [InlineData("/work-with-me")]
        [InlineData("/contact")]
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
