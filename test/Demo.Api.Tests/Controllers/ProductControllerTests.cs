using Xunit;

namespace Demo.Api.Tests.Controllers
{
    public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public HttpClient Client { get; }

        public ProductControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            Client = factory.CreateClient();
        }

        [Fact]
        public async Task Product_Test()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/api/v1.0/products");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Samsung", stringResponse);
        }
    }
}
