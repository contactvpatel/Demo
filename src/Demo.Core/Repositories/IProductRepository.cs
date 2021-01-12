using Demo.Core.Entities;
using Demo.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Models;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> Get(QueryStringParameters queryStringParameters);
        Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
    }
}
