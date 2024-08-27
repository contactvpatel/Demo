namespace Demo.Core.Models
{
    public class ProductResponseModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string ProductNumber { get; set; } = null!;
        public string Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public decimal? Weight { get; set; }
        public string ProductCategory { get; set; }
        public string ProductModel { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public DateTime? DiscontinuedDate { get; set; }
        //public byte[]? ThumbNailPhoto { get; set; }
        public string ThumbnailPhotoFileName { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ProductCategoryModel Category { get; set; }
    }

}
