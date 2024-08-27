using Demo.Core.Models;

namespace Demo.Core.Repositories
{
    public interface IAddressRepository
    {
        Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<List<CustomerAddressModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
