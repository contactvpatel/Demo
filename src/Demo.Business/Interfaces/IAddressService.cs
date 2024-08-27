using Demo.Util.FIQL;

namespace Demo.Business.Interfaces
{
    public interface IAddressService
    {
        Task<dynamic> Get(QueryParam queryParam);
    }
}
