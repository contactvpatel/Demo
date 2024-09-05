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
    public class SalesOrderHeaderRepository : Repository<SalesOrderHeaderModel>, ISalesOrderHeaderRepository
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

        public async Task<HttpResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            HttpResponseModel httpResponseModel = new();
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = _responseToDynamic.ConvertTo(retVal.Data, retVal.Responsefields ?? "");
            httpResponseModel.Data = dynamicResponse;
            httpResponseModel.TotalRecords = retVal.TotalRecords;
            return dynamicResponse;
        }

        public async Task<ListResponseToModel<SalesOrderHeaderModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<SalesOrderHeaderModel>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<SalesOrderHeaderModel>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            ListResponseToModel<SalesOrderHeaderModel> listResponseToModel = new();
            IQueryable<SalesOrderHeaderModel> result = _demoReadContext.SalesOrderHeaders
                                                            .Select(data => new SalesOrderHeaderModel()
                                                            {
                                                                SalesOrderId = data.SalesOrderId,
                                                                RevisionNumber = data.RevisionNumber,
                                                                OrderDate = data.OrderDate,
                                                                DueDate = data.DueDate,
                                                                ShipDate = data.ShipDate,
                                                                Status = data.Status,
                                                                OnlineOrderFlag = data.OnlineOrderFlag,
                                                                SalesOrderNumber = data.SalesOrderNumber,
                                                                PurchaseOrderNumber = data.PurchaseOrderNumber,
                                                                AccountNumber = data.AccountNumber,
                                                                CustomerId = data.CustomerId,
                                                                ShipToAddressId = data.ShipToAddressId,
                                                                BillToAddressId = data.BillToAddressId,
                                                                ShipMethod = data.ShipMethod,
                                                                CreditCardApprovalCode = data.CreditCardApprovalCode,
                                                                SubTotal = data.SubTotal,
                                                                TaxAmt = data.TaxAmt,
                                                                Freight = data.Freight,
                                                                TotalDue = data.TotalDue,
                                                                Comment = data.Comment,
                                                                Rowguid = data.Rowguid,
                                                                ModifiedDate = data.ModifiedDate
                                                            });

            var includes = _responseToDynamic.ParseIncludeParameter(include);
            var salesorderDetailParts = new SubQueryParam();
            ListResponseToModel<SalesOrderDetailResponse> salesOrderDetails = new();

            var customeFields = "";
            if (!string.IsNullOrEmpty(fields))
            {
                customeFields = (fields.Split(',').Any(x => "salesorderid,customerid".Split(',').Any(y => y == x.ToLower())) ? "" : "SalesOrderId,CustomerId,") + fields;
            }
            var SalesOrderHeaderResponse = await _responseToDynamic.ContextResponse<SalesOrderHeaderModel>(result, customeFields, filters, sort, pageNo, pageSize);
            List<SalesOrderHeaderModel> retVal = (JsonSerializer.Deserialize<List<SalesOrderHeaderModel>>(JsonSerializer.Serialize(SalesOrderHeaderResponse.Data))) ?? new List<SalesOrderHeaderModel>();

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

        public async Task<ListResponseToModel<SalesOrderDetailResponse>> GetSalesOrderDetail(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            fields = string.IsNullOrEmpty(fields) ? _responseToDynamic.GetPropertyNamesString<SalesOrderDetailResponse>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<SalesOrderDetailResponse>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            ListResponseToModel<SalesOrderDetailResponse> listResponseToModel = new();
            IQueryable<SalesOrderDetailResponse> result = _demoReadContext.SalesOrderDetails
                                                          .Select(data => new SalesOrderDetailResponse()
                                                          {
                                                              SalesOrderId = data.SalesOrderId,
                                                              SalesOrderDetailId = data.SalesOrderDetailId,
                                                              OrderQty = data.OrderQty,
                                                              ProductId = data.ProductId,
                                                              UnitPrice = data.UnitPrice,
                                                              UnitPriceDiscount = data.UnitPriceDiscount,
                                                              LineTotal = data.LineTotal,
                                                              Rowguid = data.Rowguid,
                                                              ModifiedDate = data.ModifiedDate,
                                                          });


            var includes = _responseToDynamic.ParseIncludeParameter(include);
            var productDetailParts = new SubQueryParam();
            ListResponseToModel<ProductResponseModel> productDetail = new();

            var customeFields = "";
            if (!string.IsNullOrEmpty(fields))
            {
                customeFields = (fields.Split(',').Any(x => "salesorderid,productid".Split(',').Any(y => y == x.ToLower())) ? "" : "ProductId,SalesOrderId") + fields;
            }
            var salesOrderDetailResponse = await _responseToDynamic.ContextResponse<SalesOrderDetailResponse>(result, customeFields, filters, sort, pageNo, pageSize);
            List<SalesOrderDetailResponse> retVal = (JsonSerializer.Deserialize<List<SalesOrderDetailResponse>>(JsonSerializer.Serialize(salesOrderDetailResponse.Data))) ?? new List<SalesOrderDetailResponse>();

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
