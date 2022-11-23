using Demo.Business.Models.Base;

namespace Demo.Business.Models
{
    public class ProductModel : BaseModel
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
        public CategoryModel Category { get; set; }
    }
}
