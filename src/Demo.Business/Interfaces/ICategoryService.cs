using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Business.Models;

namespace Demo.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetAll();
        Task<CategoryModel> GetById(int id);
    }
}
