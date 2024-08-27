using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Demo.Util.FIQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Demo.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;

        public AddressRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext,
            IConfiguration configuration,
            ILogger<ProductRepository> logger) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields ?? "");
            return dynamicResponse;
        }

        public async Task<List<CustomerAddressModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
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

            var addressResponse = await ResponseToDynamic.ContextResponse(result, fields, filters, sort, pageNo, pageSize);
            var retVal = (JsonSerializer.Deserialize<List<CustomerAddressModel>>(JsonSerializer.Serialize(addressResponse))) ?? new List<CustomerAddressModel>();
            return retVal;
        }
    }
}

