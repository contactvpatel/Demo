using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> Get(PaginationQuery paginationQuery);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
        Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<List<ProductResponseModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
