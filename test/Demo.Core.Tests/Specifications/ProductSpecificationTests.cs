using Demo.Core.Specifications;
using Demo.Core.Tests.Builders;
using Xunit;

namespace Demo.Core.Tests.Specifications
{
    public class ProductSpecificationTests
    {
        private ProductBuilder ProductBuilder { get; } = new();

        [Fact]
        public void Matches_Product_With_Category_Spec()
        {
            var spec = new ProductSpecification(ProductBuilder.ProductName1);

            var result = ProductBuilder.GetProductCollection()
                .AsQueryable()
                .FirstOrDefault(spec.Criteria);

            Assert.NotNull(result);
            Assert.Equal(ProductBuilder.ProductId1, result.ProductId);
        }
    }
}
