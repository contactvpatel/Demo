using System;
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

        public async Task<PagedList<Category>> Get(PaginationQuery paginationQuery)
        {
            var pagedData = await _demoDbContext.Categories
                .OrderBy(x => x.CategoryId)
                .Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                .Take(paginationQuery.PageSize)
                .ToListAsync();

            var totalRecords = paginationQuery.IncludeTotalCount ? await _demoDbContext.Categories.CountAsync() : 0;

            return new PagedList<Category>(pagedData, totalRecords, paginationQuery.PageNumber,
                paginationQuery.PageSize);
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