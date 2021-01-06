using System;
using Demo.Core.Entities;
using Demo.Core.Repositories;
using Demo.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly DemoContext _demoDbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(DemoContext dbContext, IConfiguration configuration, ILogger<ProductRepository> logger)
            : base(dbContext, configuration)
        {
            _demoDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Product>> GetAll()
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
            //return await _demoDbContext.Products.FromSqlRaw("SELECT ProductId, Name, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued, CategoryId FROM PRODUCT").ToListAsync();

            // Option 5: Using Repository Generic Method with QueryAsync (Dapper)
            //return await QueryAsync<Product>("SELECT ProductId, Name, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued, CategoryId FROM PRODUCT");

            // Option 6: Standard EFCore Way
            return await _demoDbContext.Products
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByName(string productName)
        {
            return await _demoDbContext.Products
                .Where(x => x.Name == productName)
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
        {
            return await _demoDbContext.Products
                .Where(x => x.CategoryId == categoryId)
                .Include(x => x.Category)
                .ToListAsync();
        }
    }
}