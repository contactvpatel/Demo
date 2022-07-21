using System.Net;
using Demo.Core.Models;
using Demo.Core.Services;
using Util.Application.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Demo.Infrastructure.Services
{
    public class SsoService : ISsoService
    {
        private readonly RestClient _client;
        private readonly IOptionsMonitor<SsoApiModel> _ssoApiModel;
        private readonly ILogger<SsoService> _logger;

        public SsoService(RestClient client, IOptionsMonitor<SsoApiModel> ssoApiModel, ILogger<SsoService> logger)
        {
            _ssoApiModel = ssoApiModel ?? throw new ArgumentNullException(nameof(ssoApiModel));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ValidateToken(string token)
        {
            var ssoApiUrl = _ssoApiModel.CurrentValue.Url;
            var endPoint = _ssoApiModel.CurrentValue.Endpoint.ValidateToken;
            var request = new RestRequest(ssoApiUrl + endPoint)
            {
                Method = Method.Post
            };
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", token);
            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
                RaiseApplicationException(response);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<SsoAuthModel> RenewToken(string token, string refreshToken)
        {
            var ssoApiUrl = _ssoApiModel.CurrentValue.Url;
            var endPoint = _ssoApiModel.CurrentValue.Endpoint.RenewToken;
            var request = new RestRequest(ssoApiUrl + endPoint)
            {
                Method = Method.Post
            };
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", token);
            request.AddBody(@"{""refreshToken"": """ + refreshToken + @"""}", "application/json");

            var response = await _client.ExecuteAsync<SsoAuthModel>(request);

            if (response.StatusCode != HttpStatusCode.OK)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<bool> Logout(string token)
        {
            var ssoApiUrl = _ssoApiModel.CurrentValue.Url;
            var endPoint = _ssoApiModel.CurrentValue.Endpoint.Logout;
            var request = new RestRequest(ssoApiUrl + endPoint)
            {
                Method = Method.Post
            };
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", token);
            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
                RaiseApplicationException(response);

            return response.StatusCode == HttpStatusCode.OK;
        }

        private void RaiseApplicationException(RestResponseBase response)
        {
            var errorMessage = response.StatusCode + "-" + response.Content;
            _logger.LogErrorExtension(errorMessage, null);
            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException(errorMessage);
            throw new ApplicationException(errorMessage);
        }
    }
}