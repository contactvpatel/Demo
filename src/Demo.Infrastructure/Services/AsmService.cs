using System.Net;
using Demo.Core.Models;
using Demo.Core.Services;
using Util.Application.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace Demo.Infrastructure.Services
{
    public class AsmService : IAsmService
    {
        private readonly RestClient _client;
        private readonly IOptionsMonitor<AsmApiModel> _asmApiModel;
        private readonly ILogger<AsmService> _logger;

        public AsmService(RestClient client, IOptionsMonitor<AsmApiModel> asmApiModel, ILogger<AsmService> logger)
        {
            _asmApiModel = asmApiModel ?? throw new ArgumentNullException(nameof(asmApiModel));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ApplicationSecurityResponseModel>> Get(
            ApplicationSecurityRequestModel applicationSecurityRequestModel)
        {
            var asmApiUrl = _asmApiModel.CurrentValue.Url;
            var endPoint = _asmApiModel.CurrentValue.Endpoint.ApplicationSecurity;
            var response =
                await Execute<AsmResponse<ApplicationSecurityResponseModel>>(asmApiUrl + endPoint,
                    applicationSecurityRequestModel);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        private async Task<T> Execute<T>(string url, ApplicationSecurityRequestModel data)
        {
            var request = new RestRequest(url)
            {
                Method = Method.Post
            };
            request.AddBody(JsonConvert.SerializeObject(data), "application/json");
            request.AddHeader("Content-type", "application/json");
            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
                if (response.Content != null)
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