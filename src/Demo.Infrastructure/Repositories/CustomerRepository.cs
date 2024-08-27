﻿using Demo.Core.Entities;
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
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IAddressRepository _addressRepository;
        private readonly ISalesOrderHeaderRepository _salesOrderHeaderRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext, IAddressRepository addressRepository, ISalesOrderHeaderRepository salesOrderHeaderRepository, IConfiguration configuration, ILogger<CustomerRepository> logger) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _salesOrderHeaderRepository = salesOrderHeaderRepository ?? throw new ArgumentNullException(nameof(salesOrderHeaderRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields ?? "");
            return dynamicResponse;
        }

        public async Task<List<CustomerModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            IQueryable<CustomerModel> result = _demoReadContext.Customers.Select(data => new CustomerModel()
            {
                CustomerId = data.CustomerId,
                CompanyName = data.CompanyName,
                EmailAddress = data.EmailAddress,
                FirstName = data.FirstName,
                LastName = data.LastName,
                MiddleName = data.MiddleName,
                ModifiedDate = DateTime.Now,
                NameStyle = data.NameStyle,
                PasswordHash = data.PasswordHash,
                PasswordSalt = data.PasswordSalt,
                Phone = data.Phone,
                Rowguid = data.Rowguid,
                SalesPerson = data.SalesPerson,
                Suffix = data.Suffix,
                Title = data.Title,
            });

            List<CustomerAddressModel> addressDetails = new List<CustomerAddressModel>();
            List<SalesOrderHeaderModel> salesOrders = new List<SalesOrderHeaderModel>();

            var foundAddressFilter = false;
            var foundSalesOrderFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var addressParts = new SubQueryParam();
            var salesorderParts = new SubQueryParam();

            /*Address Detail add*/
            if (includes.Any(x => x.ObjectName?.ToLower() == "customeraddresses"))
            {
                addressParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "customeraddresses") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(addressParts.Filters))
                {
                    foundAddressFilter = true;
                    addressDetails = await _addressRepository.Get(addressParts.Fields ?? "", addressParts.Filters ?? "");
                }
            }

            if (addressDetails.Count != 0 && foundAddressFilter)
            {
                if (string.IsNullOrEmpty(filters))
                {
                    filters = $"customerid=in=({string.Join(",", addressDetails.Select(x => x.CustomerId).ToArray())})";
                }
            }

            /*Sales Order Detail add*/
            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderheaders"))
            {
                salesorderParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "salesorderheaders") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(salesorderParts.Filters))
                {
                    foundSalesOrderFilter = true;
                    salesOrders = await _salesOrderHeaderRepository.Get(salesorderParts.Fields ?? "", salesorderParts.Filters ?? "", salesorderParts.Include ?? "");
                }
            }

            if (salesOrders.Count != 0 && foundSalesOrderFilter)
            {
                if (string.IsNullOrEmpty(filters))
                {
                    filters = (string.IsNullOrEmpty(filters) ? "" : ";") + $"customerid=in=({string.Join(",", salesOrders.Select(x => x.CustomerId).ToArray())})";
                }
            }


            var customerResponse = await ResponseToDynamic.ContextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<CustomerModel> retVal = (JsonSerializer.Deserialize<List<CustomerModel>>(JsonSerializer.Serialize(customerResponse))) ?? new List<CustomerModel>();


            if (addressDetails.Count != 0 && foundAddressFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => addressDetails.Any(y => y.CustomerId == x.CustomerId)).ToList();
                }
            }
            if (salesOrders.Count != 0 && foundSalesOrderFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => salesOrders.Any(y => y.CustomerId == x.CustomerId)).ToList();
                }
            }

            if (includes.Any(x => x.ObjectName?.ToLower() == "customeraddresses") && !foundAddressFilter && retVal.Count != 0)
            {
                addressParts.Filters = $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                addressDetails = await _addressRepository.Get(addressParts.Fields ?? "", addressParts.Filters ?? "");
            }

            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderheaders") && !foundSalesOrderFilter && retVal.Count != 0)
            {
                salesorderParts.Filters = $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                salesOrders = await _salesOrderHeaderRepository.Get(salesorderParts.Fields ?? "", salesorderParts.Filters ?? "", salesorderParts.Include ?? "");
            }

            if ((addressDetails.Count != 0 || salesOrders.Count != 0) && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.CustomerAddresses = addressDetails.Count != 0 ?
                    ResponseToDynamic.ConvertTo(addressDetails.Where(y => y.CustomerId == x.CustomerId).ToList(), addressParts.Fields ?? "") : null;
                    x.SalesOrderHeaders = salesOrders.Count != 0 ?
                    ResponseToDynamic.ConvertTo(salesOrders.Where(y => y.CustomerId == x.CustomerId).ToList(), salesorderParts.Fields ?? "") : null;
                });
            }

            return retVal;
        }
    }
}