using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Core.Communication
{
    // Inter to define contracts of Order Microservice. 
    public interface IOrderServiceProxy
    {
        Task<List<long>> GetOrders(string orderedBy);
    }
}
