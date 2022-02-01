using Xunit;

namespace Demo.Api.Tests.Controllers
{
    public class CategoryControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public HttpClient Client { get; }

        public CategoryControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        [Fact]
        public async Task Category_Test()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/api/v1.0/categories");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Phone", stringResponse);
        }
    }
}
