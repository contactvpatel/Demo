using System.Threading.Tasks;
using Demo.Business.Models;
using Demo.Core.Models;

namespace Demo.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedList<CategoryModel>> Get(PaginationQuery paginationQuery);
        Task<CategoryModel> GetById(int id);
        Task<CategoryModel> Create(CategoryModel categoryModel);
        Task Update(CategoryModel categoryModel);
    }
}
