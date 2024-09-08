using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IAddressRepository _addressRepository;
        private readonly ISalesOrderHeaderRepository _salesOrderHeaderRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly IResponseToDynamic _responseToDynamic;

        public CustomerRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext, IAddressRepository addressRepository, ISalesOrderHeaderRepository salesOrderHeaderRepository, IConfiguration configuration, ILogger<CustomerRepository> logger, IResponseToDynamic responseToDynamic) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _salesOrderHeaderRepository = salesOrderHeaderRepository ?? throw new ArgumentNullException(nameof(salesOrderHeaderRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseToDynamic = responseToDynamic;
        }

        public async Task<ResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            ResponseModel httpResponseModel = new();
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            httpResponseModel.Data = _responseToDynamic.ConvertTo(retVal.Data, retVal.Responsefields ?? "");
            httpResponseModel.TotalRecords = retVal.TotalRecords;
            return httpResponseModel;
        }

        public async Task<ResponseModelList<CustomerModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<CustomerModel>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<CustomerModel>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            if ((pageNo > 0 && pageSize > 0) && string.IsNullOrEmpty(sort))
            {
                throw new ApplicationException($"Sort parameter are required");
            }
            ResponseModelList<CustomerModel> responseModel = new()
            {
                Data = new List<CustomerModel>()
            };

            IQueryable<CustomerModel> result = _demoReadContext.Customers.Select(data => new CustomerModel()
            {
                CustomerId = data.CustomerId,
                CompanyName = data.CompanyName,
                EmailAddress = data.EmailAddress,
                FirstName = data.FirstName,
                LastName = data.LastName,
                MiddleName = data.MiddleName,
                ModifiedDate = data.ModifiedDate,
                NameStyle = data.NameStyle,
                PasswordHash = data.PasswordHash,
                PasswordSalt = data.PasswordSalt,
                Phone = data.Phone,
                Rowguid = data.Rowguid,
                SalesPerson = data.SalesPerson,
                Suffix = data.Suffix,
                Title = data.Title
            });

            List<CustomerAddressModel> addressDetails = new();
            List<SalesOrderHeaderModel> salesOrders = new();

            var includes = _responseToDynamic.ParseIncludeParameter(include);
            var addressParts = new SubQueryParam();
            var salesorderParts = new SubQueryParam();

            var customeFields = (fields.Split(',').Any(x => x.Equals("customerid", StringComparison.CurrentCultureIgnoreCase)) ? "" : "CustomerId,") + fields;
            customeFields = string.Join(",", customeFields.Split(',').Select(x => $"[{x}]").ToArray());
            var query = $"SELECT {customeFields} FROM SalesLT.Customer WITH(NOLOCK)";
            var customerResponse = await _responseToDynamic.DapperResponse<CustomerModel>(query, filters, sort, pageNo, pageSize);

            List<CustomerModel> retVal = customerResponse.Data ?? new List<CustomerModel>();

            if (includes.Any(x => x.ObjectName?.ToLower() == "customeraddresses") && retVal.Count != 0)
            {
                fields = string.Concat(fields, ",CustomerAddresses");
                addressParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "customeraddresses") ?? new SubQueryParam();

                addressParts.Filters = (string.IsNullOrEmpty(addressParts.Filters) ? "" : "(" + addressParts.Filters + ");") + $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                addressDetails = (await _addressRepository.Get(addressParts.Fields ?? "", addressParts.Filters ?? "")).Data;
            }

            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderheaders") && retVal.Count != 0)
            {
                fields = string.Concat(fields, ",SalesOrderHeaders");
                salesorderParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "salesorderheaders") ?? new SubQueryParam();

                salesorderParts.Filters = (string.IsNullOrEmpty(salesorderParts.Filters) ? "" : "(" + salesorderParts.Filters + ");") + $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                salesOrders = (await _salesOrderHeaderRepository.Get(salesorderParts.Fields ?? "", salesorderParts.Filters ?? "", salesorderParts.Include ?? "")).Data;
            }

            if ((addressDetails.Count != 0 || salesOrders.Count != 0) && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.CustomerAddresses = addressDetails.Count != 0 ?
                    _responseToDynamic.ConvertTo(addressDetails.Where(y => y.CustomerId == x.CustomerId).ToList(), addressParts.Fields ?? "") : null;
                    x.SalesOrderHeaders = salesOrders.Count != 0 ?
                    _responseToDynamic.ConvertTo(salesOrders.Where(y => y.CustomerId == x.CustomerId).ToList(), salesorderParts.Fields ?? "") : null;
                });
            }
            responseModel.Data = retVal;
            responseModel.TotalRecords = customerResponse.TotalRecords;
            responseModel.Responsefields = fields;
            return responseModel;
        }
    }
}
