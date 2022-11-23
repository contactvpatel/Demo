using Demo.Core.Entities.Base;

namespace Demo.Core.Entities
{
    public partial class Product : Entity
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int QuantityPerUnit { get; set; }

        public decimal UnitPrice { get; set; }

        public short UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }


        public static Product Create(int productId, int categoryId, string name, decimal unitPrice,
            short unitsInStock, short? unitsOnOrder, short reorderLevel,
            bool discontinued = false)
        {
            var product = new Product
            {
                ProductId = productId,
                CategoryId = categoryId,
                Name = name,
                UnitPrice = unitPrice,
                UnitsInStock = unitsInStock,
                UnitsOnOrder = unitsOnOrder,
                ReorderLevel = reorderLevel,
                Discontinued = discontinued
            };
            return product;
        }
    }
}