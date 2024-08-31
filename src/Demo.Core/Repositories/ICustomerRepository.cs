using Demo.Core.Entities;
using Demo.Core.Models;
using Demo.Core.Repositories.Base;
using Demo.Util.Models;

namespace Demo.Core.Repositories
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<HttpResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<ListResponseToModel<CustomerModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
