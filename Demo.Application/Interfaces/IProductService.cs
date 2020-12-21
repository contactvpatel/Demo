using Demo.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetAll();
        Task<ProductModel> GetById(int id);
        Task<IEnumerable<ProductModel>> GetByName(string productName);
        Task<IEnumerable<ProductModel>> GetByCategoryId(int categoryId);
        Task<ProductModel> Create(ProductModel productModel);
        Task Update(ProductModel productModel);
        Task Delete(ProductModel productModel);
    }
}
