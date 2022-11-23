using Demo.Core.Entities;
using Xunit;

namespace Demo.Core.Tests.Entities
{
    public class ProductTests
    {
        private int _testProductId = 2;
        private int _testCategoryId = 3;
        private string _testProductName = "iPhone 14";
        private decimal _testUnitPrice = 1000;
        private short _testUnitInStock = 10;
        private short _testUnitOnOrder = 2;
        private short _testReorderLevel = 2;


        [Fact]
        public void Create_Product()
        {
            var product = Product.Create(_testProductId, _testCategoryId, _testProductName, _testUnitPrice,
                _testUnitInStock, _testUnitOnOrder, _testReorderLevel);

            Assert.Equal(_testProductId, product.ProductId);
            Assert.Equal(_testCategoryId, product.CategoryId);
            Assert.Equal(_testProductName, product.Name);
            Assert.Equal(_testUnitPrice, product.UnitPrice);
            Assert.Equal(_testUnitInStock, product.UnitsInStock);
            Assert.Equal(_testUnitOnOrder, product.UnitsOnOrder);
            Assert.Equal(_testReorderLevel, product.ReorderLevel);
        }
    }
}
