using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Core.Entities;
using Demo.Core.Repositories.Base;

namespace Demo.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetById(int id);
    }
}
