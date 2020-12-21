using Demo.Core.Entities;
using Demo.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetByName(string productName);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
    }
}
