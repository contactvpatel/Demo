using Demo.Core.Communication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace Demo.Infrastructure.Communication
{
    public class Communication//<TInput, TOutput> //: ICommunication<TInput, TOutput>
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        // private readonly ILogger<Communication<TInput, TOutput>> _logger;

        //public Communication(ILogger<Communication<TInput, TOutput>> logger, IRestClient client, IRestRequest request)
        public Communication(IRestClient client, IRestRequest request)
        {
            _client = client;
            _request = request;
          //  _logger = logger;
        }

        public async Task<TOutput> Execute(TInput input, string url, Method method)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = method;
            _request.AddHeader("Content-type", "application/json");

            if(input != null)
            {
                _request.AddJsonBody(JsonConvert.SerializeObject(input));
            }

            var response = await _client.ExecuteAsync(_request);
            return JsonConvert.DeserializeObject<TOutput>(response.Content);
        }
    }
}
