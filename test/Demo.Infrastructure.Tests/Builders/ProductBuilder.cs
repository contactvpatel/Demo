﻿using Demo.Core.Entities;

namespace Demo.Infrastructure.Tests.Builders
{
    public class ProductBuilder
    {
        private Product _product;
        public int TestProductId => 5;
        public string TestProductName => "Test Product Name";
        public int TestCategoryId => 123;

        public decimal TestUnitPrice => 1.23m;
        public short TestUnitInStock => 4;
        public short TestUnitsOnOrder => 4;
        public short ReOrderLevel => 4;
        public bool Discontinued => true;

        public ProductBuilder()
        {
            _product = WithDefaultValues();

        }

        public Product Build()
        {
            return _product;
        }

        public Product WithDefaultValues()
        {
            return Product.Create(TestProductId, TestCategoryId, TestProductName, TestUnitPrice, TestUnitInStock, TestUnitsOnOrder, ReOrderLevel, Discontinued);
        }

        public Product WithAllValues()
        {
            return Product.Create(TestProductId, TestCategoryId, TestProductName, TestUnitPrice, TestUnitInStock, TestUnitsOnOrder, ReOrderLevel, Discontinued);
        }
    }
}
