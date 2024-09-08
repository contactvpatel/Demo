using Demo.Business.Models;
using Demo.Util.FIQL;
using Demo.Util.Models;

namespace Demo.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseModel> Get(QueryParam queryParam);
        Task<CustomerModel> GetById(int id);
        Task<CustomerModel> Create(CustomerModel customerModel);
        Task Update(CustomerModel customerModel);
        Task Delete(CustomerModel customerModel);
    }
}
