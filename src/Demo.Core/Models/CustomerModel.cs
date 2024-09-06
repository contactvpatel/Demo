using Demo.Core.Entities.Base;
using Demo.Util.FIQL;
using System.Text.Json.Serialization;

namespace Demo.Core.Models
{
    public class CustomerModel : Entity
    {
        [FilterMapping("CustomerId")]
        public int CustomerId { get; set; }
        [FilterMapping("NameStyle")]
        public bool NameStyle { get; set; }
        [FilterMapping("Title")]
        public string Title { get; set; }
        [FilterMapping("FirstName")]
        public string FirstName { get; set; } = null!;
        [FilterMapping("MiddleName")]
        public string MiddleName { get; set; }
        [FilterMapping("LastName")]
        public string LastName { get; set; } = null!;
        [FilterMapping("Suffix")]
        public string Suffix { get; set; }
        [FilterMapping("CompanyName")]
        public string CompanyName { get; set; }
        [FilterMapping("SalesPerson")]
        public string SalesPerson { get; set; }
        [FilterMapping("EmailAddress")]
        public string EmailAddress { get; set; }
        [FilterMapping("Phone")]
        public string Phone { get; set; }
        [FilterMapping("PasswordHash")]
        public string PasswordHash { get; set; } = null!;
        [FilterMapping("PasswordSalt")]
        public string PasswordSalt { get; set; } = null!;
        [FilterMapping("Rowguid")]
        public Guid Rowguid { get; set; }
        [FilterMapping("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
        [JsonIgnore]
        public dynamic CustomerAddresses { get; set; }
        [JsonIgnore]
        public dynamic SalesOrderHeaders { get; set; }
    }
}
