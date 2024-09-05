using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Demo.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IResponseToDynamic _responseToDynamic;

        public AddressRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext,
            IConfiguration configuration,
            ILogger<ProductRepository> logger, IResponseToDynamic responseToDynamic) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseToDynamic = responseToDynamic;
        }

        public async Task<HttpResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            HttpResponseModel httpResponseModel = new();
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = _responseToDynamic.ConvertTo(retVal.Data, retVal.Responsefields ?? "");
            httpResponseModel.Data = dynamicResponse;
            httpResponseModel.TotalRecords = retVal.TotalRecords;
            return dynamicResponse;
        }

        public async Task<ListResponseToModel<CustomerAddressModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<CustomerAddressModel>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<CustomerAddressModel>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            ListResponseToModel<CustomerAddressModel> listResponseToModel = new();
            IQueryable<CustomerAddressModel> result = _demoReadContext.CustomerAddresses
                              .Select(data => new CustomerAddressModel()
                              {
                                  CustomerId = data.CustomerId,
                                  AddressId = data.AddressId,
                                  AddressLine1 = data.Address.AddressLine1,
                                  AddressLine2 = data.Address.AddressLine2,
                                  City = data.Address.City,
                                  CountryRegion = data.Address.CountryRegion,
                                  ModifiedDate = data.Address.ModifiedDate,
                                  PostalCode = data.Address.PostalCode,
                                  Rowguid = data.Address.Rowguid,
                                  StateProvince = data.Address.StateProvince,
                              });

            var customeFields = (fields.Split(',').Any(x => x.ToLower() == "customerid") ? "" : "CustomerId,") + fields;
            customeFields = string.Join(",", CustomerAddressModelFieldsMapping.MappingFields.Where(x => customeFields.Split(',').Any(y => y.Equals(x.Key, StringComparison.CurrentCultureIgnoreCase))).Select(x => x.Value).ToArray());
            
            var query = $@"select {customeFields}
                            From SalesLT.CustomerAddress a with(nolock)
                            left join SalesLT.[Address] b with(nolock) on a.AddressID = b.AddressID";
            var addressResponse = await _responseToDynamic.DapperResponse<CustomerAddressModel>(query, filters, sort, pageNo, pageSize);

            var retVal = addressResponse.Data ?? new List<CustomerAddressModel>();
            listResponseToModel.Data = retVal;
            listResponseToModel.TotalRecords = addressResponse.TotalRecords;
            listResponseToModel.Responsefields = fields;
            return listResponseToModel;
        }
    }
}