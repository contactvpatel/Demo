using Demo.Util.FIQL;

namespace Demo.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<dynamic> Get(QueryParam queryParam);
    }
}
