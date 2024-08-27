using Demo.Business.Models;
using Demo.Core.Models;
using Demo.Util.FIQL;

namespace Demo.Business.Interfaces
{
    public interface IProductService
    {
        Task<dynamic> Get(QueryParam queryParam);
        Task<PagedList<ProductModel>> Get(PaginationQuery paginationQuery);
        Task<ProductModel> GetById(int id);
        Task<IEnumerable<ProductModel>> GetByCategoryId(int categoryId);
        Task<ProductModel> Create(ProductModel productModel);
        Task Update(ProductModel productModel);
        Task Delete(ProductModel productModel);
    }
}
