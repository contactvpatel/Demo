namespace Demo.Core.Models
{
    public class CustomerModel
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
        public dynamic CustomerAddresses { get; set; }
        public dynamic SalesOrderHeaders { get; set; }
    }
    public class CustomerResponseCustom
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
        public List<CustomerAddressModel> CustomerAddresses { get; set; } = new List<CustomerAddressModel>();
        public List<SalesOrderHeaderResponseCustom> SalesOrderHeaders { get; set; } = new List<SalesOrderHeaderResponseCustom>();
    }
}
