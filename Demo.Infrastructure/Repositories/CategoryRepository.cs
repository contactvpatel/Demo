using System.Linq;
using System.Threading.Tasks;
using Demo.Core.Entities;
using Demo.Core.Repositories;
using Demo.Core.Specifications;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Configuration;

namespace Demo.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(DemoContext dbContext, IConfiguration configuration) : base(dbContext, configuration)
        {
        }

        public async Task<Category> GetCategoryWithProducts(int categoryId)
        {
            var spec = new CategorySpecification(categoryId);
            var category = (await GetAsync(spec)).FirstOrDefault();
            return category;
        }
    }
}
