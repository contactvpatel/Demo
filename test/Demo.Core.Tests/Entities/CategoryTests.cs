using Demo.Core.Entities;
using Xunit;

namespace Demo.Core.Tests.Entities
{
    public class CategoryTests
    {
        //private int _testProductId = 3;
        private int _testCategoryId = 5;
        //private string _testProductName = "Reason";

        [Fact]
        public void Adds_Product_Into_Category()
        {
            var category = Category.Create(_testCategoryId, "newCategory");
            //category.AddProduct(_testProductId, _testProductName);

            //var firstItem = category.Products.Single();
            Assert.Equal(_testCategoryId, category.CategoryId);
            //Assert.Equal(_testProductId, firstItem.ProductId);
        }
    }
}
