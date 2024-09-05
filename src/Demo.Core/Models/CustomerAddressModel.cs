using Demo.Util.FIQL;

namespace Demo.Core.Models
{
    public partial class CustomerAddressModel
    {
        [FilterMapping("a.CustomerId")]
        public int CustomerId { get; set; }
        [FilterMapping("a.AddressId")]
        public int AddressId { get; set; }
        [FilterMapping("b.AddressLine1")]
        public string AddressLine1 { get; set; } = null!;
        [FilterMapping("b.AddressLine2")]
        public string AddressLine2 { get; set; }
        [FilterMapping("b.City")]
        public string City { get; set; } = null!;
        [FilterMapping("b.StateProvince")]
        public string StateProvince { get; set; } = null!;
        [FilterMapping("b.CountryRegion")]
        public string CountryRegion { get; set; } = null!;
        [FilterMapping("b.PostalCode")]
        public string PostalCode { get; set; } = null!;
        [FilterMapping("b.Rowguid")]
        public Guid Rowguid { get; set; }
        [FilterMapping("b.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }
    public static class CustomerAddressModelFieldsMapping
    {
        // Dictionary to map model properties to SQL column names
        public static Dictionary<string, string> MappingFields = new Dictionary<string, string>
    {
        { "CustomerId", "a.[CustomerId]" },
        { "AddressId", "a.[AddressId]" },
        { "AddressLine1", "b.[AddressLine1]" },
        { "AddressLine2", "b.[AddressLine2]" },
        { "City", "b.[City]" },
        { "StateProvince", "b.[StateProvince]" },
        { "CountryRegion", "b.[CountryRegion]" },
        { "PostalCode", "b.[PostalCode]" },
        { "Rowguid", "b.[Rowguid]" },
        { "ModifiedDate", "b.[ModifiedDate]" }
    };
    }
}
