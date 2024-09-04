using Demo.Core.Entities.Base;
using System.Text.Json.Serialization;

namespace Demo.Core.Models
{
    public class CustomerModel : Entity
    {
        public int CustomerId { get; set; }
        public bool NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long? RollNo { get; set; }
        [JsonIgnore]
        public dynamic CustomerAddresses { get; set; }
        [JsonIgnore]
        public dynamic SalesOrderHeaders { get; set; }
    }
}
