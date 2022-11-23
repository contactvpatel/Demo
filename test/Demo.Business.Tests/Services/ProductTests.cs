using Demo.Business.Services;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Core.Repositories.Base;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Demo.Business.Tests.Services
{
    public class ProductTests
    {
        // NOTE : This layer we are not loaded database objects, test functionality of business layer

        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<ILogger<ProductService>> _mockLogger;

        public ProductTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockLogger = new Mock<ILogger<ProductService>>();
        }      

        [Fact]
        public async Task Get_Product_List()
        {
            var category = Category.Create(It.IsAny<int>(), It.IsAny<string>());
            var product1 = Product.Create(It.IsAny<int>(), category.CategoryId, It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<short>(), It.IsAny<short?>(), It.IsAny<short>());
            var product2 = Product.Create(It.IsAny<int>(), category.CategoryId, It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<short>(), It.IsAny<short?>(), It.IsAny<short>());

            //category.AddProduct(product1.ProductId, It.IsAny<string>());
            //category.AddProduct(product2.ProductId, It.IsAny<string>());

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(category);
            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product1);
            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product2);

            var productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
            var productList = await productService.Get(new PaginationQuery());

            _mockProductRepository.Verify(x => x.Get(new PaginationQuery()), Times.Once);
        }

        [Fact]
        public async Task Create_New_Product_Validate_If_Exist()
        {
            var category = Category.Create(It.IsAny<int>(), It.IsAny<string>());
            var product = Product.Create(It.IsAny<int>(), category.CategoryId, It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<short>(), It.IsAny<short?>(), It.IsAny<short>());

            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockProductRepository.Setup(x => x.AddAsync(product)).ReturnsAsync(product);

            var productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);

            await Assert.ThrowsAsync<ApplicationException>(async () =>
                await productService.Create(new Models.ProductModel { ProductId = product.ProductId, CategoryId = product.CategoryId, Name = product.Name }));
        }
    }
}
