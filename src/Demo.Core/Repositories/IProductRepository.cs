using Demo.Core.Entities;
using Demo.Core.Repositories.Base;
using Demo.Core.Models;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> Get(PaginationQuery paginationQuery);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
    }
}
