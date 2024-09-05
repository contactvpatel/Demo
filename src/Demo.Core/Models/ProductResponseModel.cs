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

    public static class ProductResponseModelFieldsMapping
    {
        public static Dictionary<string, string> MappingFields = new Dictionary<string, string>
    {
        { "ProductId", "a.[ProductID]" },
        { "Name", "a.[Name]" },
        { "ProductNumber", "a.[ProductNumber]" },
        { "Color", "a.[Color]" },
        { "StandardCost", "a.[StandardCost]" },
        { "ListPrice", "a.[ListPrice]" },
        { "Size", "a.[Size]" },
        { "Weight", "a.[Weight]" },
        { "ProductCategory", "b.[Name] AS ProductCategory" },
        { "ProductModel", "c.[Name] AS ProductModel" },
        { "SellStartDate", "a.[SellStartDate]" },
        { "SellEndDate", "a.[SellEndDate]" },
        { "DiscontinuedDate", "a.[DiscontinuedDate]" },
        { "ThumbnailPhotoFileName", "a.[ThumbnailPhotoFileName]" },
        { "Rowguid", "a.[rowguid]" },
        { "ModifiedDate", "a.[ModifiedDate]" }
    };
    }

}
