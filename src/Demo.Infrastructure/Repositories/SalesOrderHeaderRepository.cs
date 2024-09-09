﻿using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Demo.Infrastructure.Repositories
{
    public class SalesOrderHeaderRepository : Repository<SalesOrderHeader>, ISalesOrderHeaderRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly IResponseToDynamic _responseToDynamic;

        public SalesOrderHeaderRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext, IProductRepository productRepository, IConfiguration configuration, ILogger<CustomerRepository> logger, IResponseToDynamic responseToDynamic) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _responseToDynamic = responseToDynamic;
        }

        public async Task<ResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            ResponseModel httpResponseModel = new();
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = _responseToDynamic.ConvertTo(retVal.Data, retVal.Responsefields ?? "");
            httpResponseModel.Data = dynamicResponse;
            httpResponseModel.TotalRecords = retVal.TotalRecords;
            return dynamicResponse;
        }

        public async Task<ResponseModelList<SalesOrderHeaderModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<SalesOrderHeaderModel>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<SalesOrderHeaderModel>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            if ((pageNo > 0 && pageSize > 0) && string.IsNullOrEmpty(sort))
            {
                throw new ApplicationException($"Sort parameter are required");
            }
            ResponseModelList<SalesOrderHeaderModel> listResponseToModel = new();
            var includes = _responseToDynamic.ParseIncludeParameter(include);
            var salesorderDetailParts = new SubQueryParam();
            ResponseModelList<SalesOrderDetailResponse> salesOrderDetails = new();

            var customeFields = _responseToDynamic.AddRequiredFields(fields,"SalesOrderId,CustomerId");
            customeFields = string.Join(",", customeFields.Split(',').Select(x => $"[{x}]").ToArray());
            var query = $@"select {customeFields} From SalesLT.SalesOrderHeader with(nolock)";
            var SalesOrderHeaderResponse = await _responseToDynamic.DapperResponse<SalesOrderHeaderModel>(query, filters, sort, pageNo, pageSize);

            List<SalesOrderHeaderModel> retVal = SalesOrderHeaderResponse.Data ?? new List<SalesOrderHeaderModel>();

            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderdetails") && retVal.Count != 0)
            {
                fields = string.Concat(fields, ",SalesOrderDetails");
                salesorderDetailParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "salesorderdetails") ?? new SubQueryParam();

                salesorderDetailParts.Filters = (string.IsNullOrEmpty(salesorderDetailParts.Filters) ? "" : "(" + salesorderDetailParts.Filters + ");") + $"salesorderid=in=({string.Join(",", retVal.Select(x => x.SalesOrderId).ToArray())})";
                salesOrderDetails = await GetSalesOrderDetail(salesorderDetailParts.Fields ?? "", salesorderDetailParts.Filters ?? "", salesorderDetailParts.Include ?? "");
            }

            if (salesOrderDetails.Data.Count != 0 && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.SalesOrderDetails = _responseToDynamic.ConvertTo(salesOrderDetails.Data.Where(y => y.SalesOrderId == x.SalesOrderId).ToList(), salesorderDetailParts.Fields ?? "");
                });
            }
            listResponseToModel.Data = retVal;
            listResponseToModel.TotalRecords = SalesOrderHeaderResponse.TotalRecords;
            listResponseToModel.Responsefields = fields;
            return listResponseToModel;
        }

        public async Task<ResponseModelList<SalesOrderDetailResponse>> GetSalesOrderDetail(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<SalesOrderDetailResponse>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<SalesOrderDetailResponse>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            ResponseModelList<SalesOrderDetailResponse> listResponseToModel = new();
            var includes = _responseToDynamic.ParseIncludeParameter(include);
            var productDetailParts = new SubQueryParam();
            ResponseModelList<ProductResponseModel> productDetail = new();

            var customeFields = _responseToDynamic.AddRequiredFields(fields, "SalesOrderId,ProductId");
            customeFields = string.Join(",", customeFields.Split(',').Select(x => $"[{x}]").ToArray());
            var query = $@"select {customeFields} From SalesLT.SalesOrderDetail with(nolock)";
            var salesOrderDetailResponse = await _responseToDynamic.DapperResponse<SalesOrderDetailResponse>(query, filters, sort, pageNo, pageSize);

            List<SalesOrderDetailResponse> retVal = salesOrderDetailResponse.Data ?? new List<SalesOrderDetailResponse>();

            if (includes.Any(x => x.ObjectName?.ToLower() == "product") && retVal.Count != 0)
            {
                fields = string.Concat(fields, ",Product");
                productDetailParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "product") ?? new SubQueryParam();

                productDetailParts.Filters = (string.IsNullOrEmpty(productDetailParts.Filters) ? "" : "(" + productDetailParts.Filters + ");") + $"productid=in=({string.Join(",", retVal.Select(x => x.ProductId).ToArray())})";
                productDetail = await _productRepository.Get(productDetailParts.Fields ?? "", productDetailParts.Filters ?? "", productDetailParts.Include ?? "");
            }

            if (productDetail.Data.Count != 0 && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.Product = _responseToDynamic.ConvertTo(productDetail.Data.Where(y => y.ProductId == x.ProductId).FirstOrDefault(), productDetailParts.Fields ?? "");
                });
            }
            listResponseToModel.Data = retVal;
            listResponseToModel.TotalRecords = salesOrderDetailResponse.TotalRecords;
            listResponseToModel.Responsefields = fields;
            return listResponseToModel;
        }
    }
}
