using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Core.Specifications;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly DemoReadContext _demoReadContext;
        private readonly DemoWriteContext _demoWriteContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(DemoReadContext demoReadContext, DemoWriteContext demoWriteContext,
            IConfiguration configuration,
            ILogger<CategoryRepository> logger) : base(demoReadContext, demoWriteContext, configuration)
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

        public async Task<PagedList<Category>> Get(PaginationQuery paginationQuery)
        {
            return null;
            //var pagedData = await _demoReadContext.Categories
            //    .OrderBy(x => x.CategoryId)
            //    .Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
            //    .Take(paginationQuery.PageSize)
            //    .ToListAsync();

            //var totalRecords = paginationQuery.IncludeTotalCount ? await _demoReadContext.Categories.CountAsync() : 0;

            //return new PagedList<Category>(pagedData, totalRecords, paginationQuery.PageNumber,
            //    paginationQuery.PageSize);
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