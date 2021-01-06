using System.Collections.Generic;
using Demo.Core.Entities;

namespace Demo.Core.Tests.Builders
{
    public class ProductBuilder
    {
        public int ProductId1 => 123;
        public int ProductId2 => 124;
        public int ProductId3 => 125;
        public string ProductName1 => "ProductX";
        public string ProductName2 => "ProductY";
        public string ProductName3 => "ProductZ";

        public List<Product> GetProductCollection()
        {
            return new()
            {
                Product.Create(ProductId1, 1, ProductName1),
                Product.Create(ProductId2, 1, ProductName2),
                Product.Create(ProductId3, 1, ProductName3)
            };
        }
    }
}
