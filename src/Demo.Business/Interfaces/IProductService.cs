using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Business.Models;
using Demo.Core.Models;

namespace Demo.Business.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<ProductModel>> Get(QueryStringParameters queryStringParameters);
        Task<ProductModel> GetById(int id);
        Task<IEnumerable<ProductModel>> GetByCategoryId(int categoryId);
        Task<ProductModel> Create(ProductModel productModel);
        Task Update(ProductModel productModel);
        Task Delete(ProductModel productModel);
    }
}
