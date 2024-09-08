using Demo.Util.FIQL;
using Demo.Util.Models;

namespace Demo.Business.Interfaces
{
    public interface IProductService
    {
        Task<ResponseModel> Get(QueryParam queryParam);
    }
}
