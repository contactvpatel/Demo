using Xunit;

namespace Demo.Api.Tests.Controllers
{
    public class CategoryControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public HttpClient Client { get; }

        public CategoryControllerTest(CustomWebApplicationFactory<Program> factory)
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
