using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories;
using Demo.Infrastructure.Tests.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Infrastructure.Tests.Repositories
{
    public class ProductTests
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;

        private readonly ProductRepository _productRepository;
        private readonly ITestOutputHelper _output;
        private ProductBuilder ProductBuilder { get; } = new();
        private CategoryBuilder CategoryBuilder { get; } = new();

        public ProductTests(ITestOutputHelper output)
        {
            _output = output;

            var dbReadOptions = new DbContextOptionsBuilder<DemoContext>()
                .UseInMemoryDatabase(databaseName: "Demo")
                .Options;

            var dbWriteOptions = new DbContextOptionsBuilder<DemoContext>()
                .UseInMemoryDatabase(databaseName: "Demo")
                .Options;

            _demoReadContext = new DemoReadContext(dbReadOptions);
            _demoWriteContext = new DemoWriteContext(dbWriteOptions);

            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<ProductRepository>>();
            _productRepository = new ProductRepository(_demoReadContext, _demoWriteContext, mockConfiguration.Object,
                mockLogger.Object);
        }

        [Fact]
        public async Task Get_Existing_Product()
        {
            var existingProduct = ProductBuilder.WithDefaultValues();
            var product = _demoWriteContext.Products.SingleOrDefault(f => f.Name == existingProduct.Name);

            if (product != null)
            {
                _demoWriteContext.Products.Remove(product);
                await _demoWriteContext.SaveChangesAsync();
            }

            await _demoWriteContext.Products.AddAsync(existingProduct);
            await _demoWriteContext.SaveChangesAsync();

            var productId = existingProduct.ProductId;
            _output.WriteLine($"ProductId: {productId}");

            var productFromRepo = await _productRepository.GetByIdAsync(productId);
            Assert.Equal(ProductBuilder.TestProductId, productFromRepo.ProductId);
            Assert.Equal(ProductBuilder.TestCategoryId, productFromRepo.CategoryId);
        }

        [Fact]
        public async Task Get_Product_By_Category()
        {
            var existingCategory = CategoryBuilder.WithDefaultValues();

            var category = _demoWriteContext.Categories.SingleOrDefault(f => f.Name == existingCategory.Name);

            if (category != null)
            {
                _demoWriteContext.Categories.Remove(category);
                await _demoWriteContext.SaveChangesAsync();
            }

            await _demoWriteContext.Categories.AddAsync(existingCategory);

            var existingProduct = ProductBuilder.WithDefaultValues();
            var product = _demoWriteContext.Products.SingleOrDefault(f => f.Name == existingProduct.Name);

            if (product != null)
            {
                _demoWriteContext.Products.Remove(product);
                await _demoWriteContext.SaveChangesAsync();
            }

            await _demoWriteContext.Products.AddAsync(existingProduct);
            await _demoWriteContext.SaveChangesAsync();
            var categoryId = existingProduct.CategoryId;
            _output.WriteLine($"CategoryId: {categoryId}");

            var productListFromRepo = await _productRepository.GetByCategoryId(categoryId);
            Assert.Equal(ProductBuilder.TestCategoryId, productListFromRepo.ToList().First().CategoryId);
        }
    }
}
