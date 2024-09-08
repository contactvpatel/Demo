using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;
using Demo.Util.Models;

namespace Demo.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<ResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<ResponseModelList<ProductResponseModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
