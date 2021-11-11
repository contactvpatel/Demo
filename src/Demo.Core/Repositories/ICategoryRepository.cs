using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;

namespace Demo.Core.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedList<Category>> Get(PaginationQuery paginationQuery);
        Task<Category> GetById(int id);
    }
}
