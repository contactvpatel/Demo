using Demo.Core.Models;

namespace Demo.Core.Repositories
{
    public interface ISalesOrderHeaderRepository
    {
        Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<List<SalesOrderHeaderModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
