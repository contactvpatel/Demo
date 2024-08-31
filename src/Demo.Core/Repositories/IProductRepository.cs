using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;
using Demo.Util.Models;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> Get(PaginationQuery paginationQuery);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
        Task<HttpResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<ListResponseToModel<ProductResponseModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
