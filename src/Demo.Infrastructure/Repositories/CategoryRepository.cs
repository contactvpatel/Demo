using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Core.Specifications;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly DemoContext _demoDbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(DemoContext dbContext, IConfiguration configuration,
            ILogger<CategoryRepository> logger) : base(dbContext, configuration)
        {
            _demoDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedList<Category>> Get(QueryStringParameters queryStringParameters)
        {
            var pagedData = await _demoDbContext.Categories
                .Skip((queryStringParameters.PageNumber - 1) * queryStringParameters.PageSize)
                .Take(queryStringParameters.PageSize)
                .ToListAsync();

            var totalRecords = await _demoDbContext.Products.CountAsync();

            return new PagedList<Category>(pagedData, totalRecords, queryStringParameters.PageNumber,
                queryStringParameters.PageSize);
        }

        public async Task<Category> GetById(int id)
        {
            // Using Specification Pattern
            var spec = new CategorySpecification(id);
            var category = (await GetAsync(spec)).FirstOrDefault();
            return category;
        }
    }
}