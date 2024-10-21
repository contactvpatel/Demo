using Demo.Core.Entities;
using Demo.Core.Entities.Base;
using Demo.Util.FIQL;
using System.Text.Json.Serialization;

namespace Demo.Core.Models
{
    public class CustomerModel : Entity
    {
        [FilterMapping("Customer.CustomerId")]
        public int CustomerId { get; set; }
        [FilterMapping("Customer.NameStyle")]
        public bool NameStyle { get; set; }
        [FilterMapping("Customer.Title")]
        public string Title { get; set; }
        [FilterMapping("Customer.FirstName")]
        public string FirstName { get; set; } = null!;
        [FilterMapping("Customer.MiddleName")]
        public string MiddleName { get; set; }
        [FilterMapping("Customer.LastName")]
        public string LastName { get; set; } = null!;
        [FilterMapping("Customer.Suffix")]
        public string Suffix { get; set; }
        [FilterMapping("Customer.CompanyName")]
        public string CompanyName { get; set; }
        [FilterMapping("Customer.SalesPerson")]
        public string SalesPerson { get; set; }
        [FilterMapping("Customer.EmailAddress")]
        public string EmailAddress { get; set; }
        [FilterMapping("Customer.Phone")]
        public string Phone { get; set; }
        [FilterMapping("Customer.PasswordHash")]
        public string PasswordHash { get; set; } = null!;
        [FilterMapping("Customer.PasswordSalt")]
        public string PasswordSalt { get; set; } = null!;
        [FilterMapping("Customer.Rowguid")]
        public Guid Rowguid { get; set; }
        [FilterMapping("Customer.ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
        [JsonIgnore]
        [FilterMapping(typeof(CustomerAddressModel))]
        public dynamic CustomerAddresses { get; set; }
        [JsonIgnore]
        [FilterMapping(typeof(SalesOrderFilterModel))]
        public dynamic SalesOrderHeaders { get; set; }
    }
}
