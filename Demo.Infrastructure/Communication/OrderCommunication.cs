using Demo.Core.Communication;
using Demo.Core.Entities;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Infrastructure.Communication
{
    public class OrderCommunication : IOrderCommunication
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;

        public OrderCommunication(IRestClient client, IRestRequest request)
        {
            _client = client;
            _request = request;
        }

        public async Task<List<long>> GetOrders(string orderedBy)
        {
            var orderServiceUrl = ""; // Should get the URL from the configuration and append the required API names i.e. http://orderService/api/get
            var orderInput = new OrderInput() { OrderBy = orderedBy };
            var response = await Execute(JsonConvert.SerializeObject(orderInput), orderServiceUrl, Method.GET);
            var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(response);
            return orderResponse.Orders;
        }

        private async Task<string> Execute(string input, string url, Method method)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = method;
            _request.AddHeader("Content-type", "application/json");

            if (!string.IsNullOrEmpty(input))
            {
                _request.AddJsonBody(input);
            }

            var response = await _client.ExecuteAsync(_request);
            return response.Content;
        }
    }
}
