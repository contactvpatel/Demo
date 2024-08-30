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

        /* DbContext
         
         _demoReadContext -> Use for all read operation (Get)
         _demoWriteContext -> Use for all write operation (Create, Update. Delete)

         */

        public async Task<PagedList<Product>> Get(PaginationQuery paginationQuery)
        {
            // Option 1: Using Specification & Generic Repository (EFCore)
            //var spec = new ProductSpecification();
            //return await GetAsync(spec);

            // Option 2: Using Repository Generic Method - GetAllAsync (EFCore)
            //return await GetAllAsync();

            // Option 3: Using Repository Generic Method with Category Include and Disable Tracking - GetAsync Overloaded Method (EFCore)
            //return await GetAsync(null, null, "Category", true);

            // Option 4: Using Repository Generic Method with Category Include, Disable Tracking & Order By Product Name - GetAsync Overloaded Method (EFCore)
            //return await GetAsync(null, o => o.OrderBy(s => s.Name), "Category", true);

            // Option 5: Using Raw SQL (EFCore)
            //return await _demoReadContext.Products.FromSqlRaw("SELECT ProductId, Name, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued, CategoryId FROM PRODUCT").ToListAsync();

            // Option 5: Using Repository Generic Method with QueryAsync (Dapper)
            // return await QueryAsync<Product>("SELECT ProductId, Name, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued, CategoryId FROM PRODUCT");

            // Option 6: Standard EFCore Way

            return null;

            //var pagedData = await _demoReadContext.Products
            //    .Include(x => x.Category)
            //    .OrderBy(x => x.ProductId)
            //    .Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
            //    .Take(paginationQuery.PageSize)
            //    .ToListAsync();

            //var totalRecords = paginationQuery.IncludeTotalCount ? await _demoReadContext.Products.CountAsync() : 0;

            //return new PagedList<Product>(pagedData, totalRecords, paginationQuery.PageNumber,
            //    paginationQuery.PageSize);
        }

        public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
        {
            //return await _demoReadContext.Products
            //    .Where(x => x.CategoryId == categoryId)
            //    .Include(x => x.Category)
            //    .ToListAsync();
            return null;
        }

        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields ?? "", filters ?? "", include ?? "", sort ?? "", pageNo, pageSize);
            dynamic dynamicResponse = _responseToDynamic.ConvertTo(retVal, fields ?? "");
            return dynamicResponse;
        }

        public async Task<List<ProductResponseModel>> Get(string fields, string filters, string include, string sort, int pageNo, int pageSize)
        {
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
            var productResponse = await _responseToDynamic.ContextResponse(result, fields, filters, sort, pageNo, pageSize);
            var retVal = (JsonSerializer.Deserialize<List<ProductResponseModel>>(JsonSerializer.Serialize(productResponse))) ?? new List<ProductResponseModel>();
            return retVal;
        }
    }
}
