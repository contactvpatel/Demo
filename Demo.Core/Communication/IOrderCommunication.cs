using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Core.Communication
{
    // Inter to define contracts of Order Microservice. 
    public interface IOrderCommunication
    {
        Task<List<long>> GetOrders(string orderedBy);
    }
}
