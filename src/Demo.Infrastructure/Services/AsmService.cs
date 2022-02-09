using System.Net;
using Demo.Core.Models;
using Demo.Core.Services;
using Demo.Util.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Demo.Infrastructure.Services
{
    public class AsmService : IAsmService
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        private readonly IOptions<AsmApiModel> _asmApiModel;
        private readonly ILogger<AsmService> _logger;

        public AsmService(IRestClient client, IOptions<AsmApiModel> asmApiModel, ILogger<AsmService> logger)
        {
            _asmApiModel = asmApiModel ?? throw new ArgumentNullException(nameof(asmApiModel));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _request = new RestRequest();
        }

        public async Task<IEnumerable<ApplicationSecurityResponseModel>> Get(
            ApplicationSecurityRequestModel applicationSecurityRequestModel)
        {
            var asmApiUrl = _asmApiModel.Value.Url;
            var endPoint = _asmApiModel.Value.Endpoint.ApplicationSecurity;
            var response =
                await Execute<AsmResponse<ApplicationSecurityResponseModel>>(asmApiUrl + endPoint,
                    applicationSecurityRequestModel);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        private async Task<T> Execute<T>(string url, ApplicationSecurityRequestModel data)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = Method.POST;
            _request.AddJsonBody(JsonConvert.SerializeObject(data));
            _request.AddHeader("Content-type", "application/json");
            var response = await _client.ExecuteAsync(_request);

            if (response.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(response.Content);

            throw new ApplicationException(response.Content);
        }

        private void RaiseApplicationException<T>(AsmResponse<T> response)
        {
            var errorMessage = response.Errors?.FirstOrDefault()?.StatusCode + "-" +
                               response.Errors?.FirstOrDefault()?.ErrorId + "-" +
                               response.Errors?.FirstOrDefault()?.Message;
            _logger.LogErrorExtension(errorMessage, null);
            throw new ApplicationException(errorMessage);
        }
    }
}