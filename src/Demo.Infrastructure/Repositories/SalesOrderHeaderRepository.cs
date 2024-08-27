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
    public class SalesOrderHeaderRepository : Repository<SalesOrderHeaderModel>, ISalesOrderHeaderRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerRepository> _logger;

        public SalesOrderHeaderRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext, IProductRepository productRepository, IConfiguration configuration, ILogger<CustomerRepository> logger) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields ?? "");
            return dynamicResponse;
        }

        public async Task<List<SalesOrderHeaderModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
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
            var foundSalesOrderFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var salesorderDetailParts = new SubQueryParam();
            List<SalesOrderDetailResponse> salesOrderDetails = new List<SalesOrderDetailResponse>();

            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderdetails"))
            {
                salesorderDetailParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "salesorderdetails") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(salesorderDetailParts.Filters))
                {
                    foundSalesOrderFilter = true;
                    salesOrderDetails = await GetSalesOrderDetail(salesorderDetailParts.Fields ?? "", salesorderDetailParts.Filters ?? "", salesorderDetailParts.Include ?? "");
                }
            }

            if (salesOrderDetails.Count != 0 && foundSalesOrderFilter)
            {
                if (string.IsNullOrEmpty(filters))
                {
                    filters = $"salesorderid=in=({string.Join(",", salesOrderDetails.Select(x => x.SalesOrderId).ToArray())})";
                }
            }

            var SalesOrderHeaderResponse = await ResponseToDynamic.ContextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<SalesOrderHeaderModel> retVal = (JsonSerializer.Deserialize<List<SalesOrderHeaderModel>>(JsonSerializer.Serialize(SalesOrderHeaderResponse))) ?? new List<SalesOrderHeaderModel>();

            if (salesOrderDetails.Count != 0 && foundSalesOrderFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => salesOrderDetails.Any(y => y.SalesOrderId == x.SalesOrderId)).ToList();
                }
            }

            if (includes.Any(x => x.ObjectName?.ToLower() == "salesorderdetails") && !foundSalesOrderFilter && retVal.Count != 0)
            {
                salesorderDetailParts.Filters = $"salesorderid=in=({string.Join(",", retVal.Select(x => x.SalesOrderId).ToArray())})";
                salesOrderDetails = await GetSalesOrderDetail(salesorderDetailParts.Fields ?? "", salesorderDetailParts.Filters ?? "", salesorderDetailParts.Include ?? "");
            }

            if (salesOrderDetails.Count != 0 && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.SalesOrderDetails = ResponseToDynamic.ConvertTo(salesOrderDetails.Where(y => y.SalesOrderId == x.SalesOrderId).ToList(), salesorderDetailParts.Fields ?? "");
                });
            }

            return retVal;
        }

        public async Task<List<SalesOrderDetailResponse>> GetSalesOrderDetail(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
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


            var foundProductDetailFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var productDetailParts = new SubQueryParam();
            List<ProductResponseModel> productDetail = new List<ProductResponseModel>();

            if (includes.Any(x => x.ObjectName?.ToLower() == "product"))
            {
                productDetailParts = includes.FirstOrDefault(x => x.ObjectName?.ToLower() == "product") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(productDetailParts.Filters))
                {
                    foundProductDetailFilter = true;
                    productDetail = await _productRepository.Get(productDetailParts.Fields ?? "", productDetailParts.Filters ?? "", productDetailParts.Include ?? "");
                }
            }

            if (productDetail.Count != 0 && foundProductDetailFilter)
            {
                if (string.IsNullOrEmpty(filters))
                {
                    filters = $"productid=in=({string.Join(",", productDetail.Select(x => x.ProductId).ToArray())})";
                }
            }

            var salesOrderDetailResponse = await ResponseToDynamic.ContextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<SalesOrderDetailResponse> retVal = (JsonSerializer.Deserialize<List<SalesOrderDetailResponse>>(JsonSerializer.Serialize(salesOrderDetailResponse))) ?? new List<SalesOrderDetailResponse>();

            if (productDetail.Count != 0 && foundProductDetailFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => productDetail.Any(y => y.ProductId == x.ProductId)).ToList();
                }
            }

            if (includes.Any(x => x.ObjectName?.ToLower() == "product") && !foundProductDetailFilter && retVal.Count != 0)
            {
                productDetailParts.Filters = $"productid=in=({string.Join(",", retVal.Select(x => x.ProductId).ToArray())})";
                productDetail = await _productRepository.Get(productDetailParts.Fields ?? "", productDetailParts.Filters ?? "", productDetailParts.Include ?? "");
            }

            if (productDetail.Count != 0 && retVal.Count != 0)
            {
                retVal.ForEach(x =>
                {
                    x.Product = ResponseToDynamic.ConvertTo(productDetail.Where(y => y.ProductId == x.ProductId).ToList(), productDetailParts.Fields ?? "");
                });
            }
            return retVal;
        }
    }
}
