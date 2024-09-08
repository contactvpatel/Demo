using Demo.Core.Entities;
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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IResponseToDynamic _responseToDynamic;

        public ProductRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext,
            IConfiguration configuration,
            ILogger<ProductRepository> logger, IResponseToDynamic responseToDynamic) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
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

        public async Task<ResponseModelList<ProductResponseModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
            fields = string.IsNullOrEmpty(fields) ?  _responseToDynamic.GetPropertyNamesString<ProductResponseModel>() : fields;
            if (!_responseToDynamic.TryGetMissingPropertyNames<ProductResponseModel>(fields, out var missingFields))
            {
                throw new ApplicationException($"{missingFields} column not found");
            }
            ResponseModelList<ProductResponseModel> listResponseToModel = new();
            IQueryable<ProductResponseModel> result = _demoReadContext.Products
                                                .Select(data => new ProductResponseModel()
                                                {
                                                    ProductId = data.ProductId,
                                                    Name = data.Name,
                                                    ProductNumber = data.ProductNumber,
                                                    Color = data.Color,
                                                    StandardCost = data.StandardCost,
                                                    ListPrice = data.ListPrice,
                                                    Size = data.Size,
                                                    Weight = data.Weight,
                                                    ProductCategory = data.ProductCategory.Name,
                                                    ProductModel = data.ProductModel.Name,
                                                    SellStartDate = data.SellStartDate,
                                                    SellEndDate = data.SellEndDate,
                                                    DiscontinuedDate = data.DiscontinuedDate,
                                                    ThumbnailPhotoFileName = data.ThumbnailPhotoFileName,
                                                    Rowguid = data.Rowguid,
                                                    ModifiedDate = data.ModifiedDate,
                                                });
            var productResponse = await _responseToDynamic.ContextResponse<ProductResponseModel>(result, fields, filters, sort, pageNo, pageSize);
            var retVal = (JsonSerializer.Deserialize<List<ProductResponseModel>>(JsonSerializer.Serialize(productResponse.Data))) ?? new List<ProductResponseModel>();
            listResponseToModel.Data = retVal;
            listResponseToModel.TotalRecords = productResponse.TotalRecords;
            listResponseToModel.Responsefields = fields;
            return listResponseToModel;
        }
    }
}
