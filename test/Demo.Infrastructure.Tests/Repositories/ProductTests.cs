using System.Linq;
using System.Threading.Tasks;
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
        private readonly DemoContext _demoDbContext;
        private readonly ProductRepository _productRepository;
        private readonly ITestOutputHelper _output;
        private ProductBuilder ProductBuilder { get; } = new();
        private CategoryBuilder CategoryBuilder { get; } = new();

        public ProductTests(ITestOutputHelper output)
        {
            _output = output;
            
            var dbOptions = new DbContextOptionsBuilder<DemoContext>()
                .UseInMemoryDatabase(databaseName: "Demo")
                .Options;
            _demoDbContext = new DemoContext(dbOptions);

            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<ProductRepository>>();
            _productRepository = new ProductRepository(_demoDbContext, mockConfiguration.Object, mockLogger.Object);
        }

        [Fact]
        public async Task Get_Existing_Product()
        {
            var existingProduct = ProductBuilder.WithDefaultValues();
            var product = _demoDbContext.Products.SingleOrDefault(f => f.Name == existingProduct.Name);

            if (product != null)
            {
                _demoDbContext.Products.Remove(product);
                await _demoDbContext.SaveChangesAsync();
            }
            
            await _demoDbContext.Products.AddAsync(existingProduct);           
            await _demoDbContext.SaveChangesAsync();

            var productId = existingProduct.ProductId;
            _output.WriteLine($"ProductId: {productId}");

            var productFromRepo = await _productRepository.GetByIdAsync(productId);
            Assert.Equal(ProductBuilder.TestProductId, productFromRepo.ProductId);
            Assert.Equal(ProductBuilder.TestCategoryId, productFromRepo.CategoryId);
        }

        [Fact]
        public async Task Get_Product_By_Name()
        {
            // GetProductByNameAsync spec required Category, because it is included Category entity so it should be exist
            var existingCategory = CategoryBuilder.WithDefaultValues();

            var category = _demoDbContext.Categories.SingleOrDefault(f => f.Name == existingCategory.Name);

            if (category != null)
            {
                _demoDbContext.Categories.Remove(category);
                await _demoDbContext.SaveChangesAsync();
            }

            await _demoDbContext.Categories.AddAsync(existingCategory);

            var existingProduct = ProductBuilder.WithDefaultValues();
            var product = _demoDbContext.Products.SingleOrDefault(f => f.Name == existingProduct.Name);

            if (product != null)
            {
                _demoDbContext.Products.Remove(product);
                await _demoDbContext.SaveChangesAsync();
            }
            
            await _demoDbContext.Products.AddAsync(existingProduct);
            
            await _demoDbContext.SaveChangesAsync();
            
            var productName = existingProduct.Name;
            _output.WriteLine($"ProductName: {productName}");
            
            var productListFromRepo = await _productRepository.GetByName(productName);
            Assert.Equal(ProductBuilder.TestProductName, productListFromRepo.ToList().First().Name);
        }

        [Fact]
        public async Task Get_Product_By_Category()
        {
            var existingCategory = CategoryBuilder.WithDefaultValues();

            var category = _demoDbContext.Categories.SingleOrDefault(f => f.Name == existingCategory.Name);

            if (category != null)
            {
                _demoDbContext.Categories.Remove(category);
                await _demoDbContext.SaveChangesAsync();
            }

            await _demoDbContext.Categories.AddAsync(existingCategory);

            var existingProduct = ProductBuilder.WithDefaultValues();
            var product = _demoDbContext.Products.SingleOrDefault(f => f.Name == existingProduct.Name);

            if (product != null)
            {
                _demoDbContext.Products.Remove(product);
                await _demoDbContext.SaveChangesAsync();
            }

            await _demoDbContext.Products.AddAsync(existingProduct);
            await _demoDbContext.SaveChangesAsync();
            var categoryId = existingProduct.CategoryId;
            _output.WriteLine($"CategoryId: {categoryId}");

            var productListFromRepo = await _productRepository.GetByCategoryId(categoryId);
            Assert.Equal(ProductBuilder.TestCategoryId, productListFromRepo.ToList().First().CategoryId);
        }
    }
}
