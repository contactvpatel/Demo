using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Core.Communication
{
    public interface IOrderCommunication
    {
        Task<List<long>> GetOrders(string orderedBy);
    }
}
