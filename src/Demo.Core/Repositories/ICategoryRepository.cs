using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;

namespace Demo.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedList<Category>> Get(QueryStringParameters queryStringParameters);
        Task<Category> GetById(int id);
    }
}
