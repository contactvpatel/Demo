using Demo.Core.Entities;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using Demo.Core.Models;
using Demo.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext,
            IConfiguration configuration,
            ILogger<ProductRepository> logger) : base(demoReadContext, demoWriteContext, configuration)
        {
            _demoReadContext = demoReadContext ?? throw new ArgumentNullException(nameof(demoReadContext));
            _demoWriteContext = demoWriteContext ?? throw new ArgumentNullException(nameof(demoWriteContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var pagedData = await _demoReadContext.Products
                .Include(x => x.Category)
                .OrderBy(x => x.ProductId)
                .Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                .Take(paginationQuery.PageSize)
                .ToListAsync();

            var totalRecords = paginationQuery.IncludeTotalCount ? await _demoReadContext.Products.CountAsync() : 0;

            return new PagedList<Product>(pagedData, totalRecords, paginationQuery.PageNumber,
                paginationQuery.PageSize);
        }

        public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
        {
            return await _demoReadContext.Products
                .Where(x => x.CategoryId == categoryId)
                .Include(x => x.Category)
                .ToListAsync();
        }
    }
}