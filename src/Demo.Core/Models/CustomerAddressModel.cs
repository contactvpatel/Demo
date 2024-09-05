namespace Demo.Core.Models
{
    public partial class CustomerAddressModel
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; } = null!;
        public string AddressLine2 { get; set; }
        public string City { get; set; } = null!;
        public string StateProvince { get; set; } = null!;
        public string CountryRegion { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public Guid Rowguid { get; set; }
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
