using Demo.Util.FIQL;

namespace Demo.Core.Models
{

    public class ProductResponseModel
    {
        [FilterMapping("a.ProductID")]
        public int ProductId { get; set; }

        [FilterMapping("a.[Name]")]
        public string Name { get; set; } = null!;

        [FilterMapping("a.ProductNumber")]
        public string ProductNumber { get; set; } = null!;

        [FilterMapping("a.Color")]
        public string Color { get; set; }

        [FilterMapping("a.StandardCost")]
        public decimal StandardCost { get; set; }

        [FilterMapping("a.ListPrice")]
        public decimal ListPrice { get; set; }

        [FilterMapping("a.Size")]
        public string Size { get; set; }

        [FilterMapping("a.[Weight]")]
        public decimal? Weight { get; set; }

        [FilterMapping("b.[Name]")]
        public string ProductCategory { get; set; }

        [FilterMapping("c.[Name]")]
        public string ProductModel { get; set; }

        [FilterMapping("a.SellStartDate")]
        public DateTime SellStartDate { get; set; }

        [FilterMapping("a.SellEndDate")]
        public DateTime? SellEndDate { get; set; }

        [FilterMapping("a.DiscontinuedDate")]
        public DateTime? DiscontinuedDate { get; set; }

        [FilterMapping("a.ThumbnailPhotoFileName")]
        public string ThumbnailPhotoFileName { get; set; }

        [FilterMapping("a.rowguid")]
        public Guid Rowguid { get; set; }

        [FilterMapping("a.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
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
