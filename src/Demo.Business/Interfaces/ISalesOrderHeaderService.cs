using Demo.Util.FIQL;

namespace Demo.Business.Interfaces
{
    public interface ISalesOrderHeaderService
    {
        Task<dynamic> Get(QueryParam queryParam);
    }
}
