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
    public class MisService : IMisService
    {
        private readonly RestClient _client;
        private readonly IOptionsMonitor<MisApiModel> _misApiModel;
        private readonly ILogger<MisService> _logger;

        public MisService(RestClient client, IOptionsMonitor<MisApiModel> misApiModel, ILogger<MisService> logger)
        {
            _misApiModel = misApiModel ?? throw new ArgumentNullException(nameof(misApiModel));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DepartmentModel>> GetAllDepartments(int divisionId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Department;
            var departmentServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<DepartmentModel>>(departmentServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<DepartmentModel> GetDepartmentById(int id)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Department;
            var departmentServiceUrl = misApiUrl + endPoint + "?deptId=" + id;
            var response = await Execute<MisResponse<DepartmentModel>>(departmentServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<EntityModel>> GetEntityByDivision(int divisionId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Entity;
            var entityServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<EntityModel>>(entityServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleTypeModel>> GetAllRoleTypes(int divisionId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.RoleType;
            var roleTypeServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<RoleTypeModel>>(roleTypeServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleModel>> GetAllRoles(int divisionId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<RoleModel> GetRoleById(int id)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?roleId=" + id;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByDepartmentId(int departmentId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?deptId=" + departmentId;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByDepartments(int[] departmentIds)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint;

            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl, "deptId", departmentIds);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<PositionModel>> GetAllPositions(int divisionId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Position;
            var positionServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<PositionModel>>(positionServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<PositionModel>> GetPositions(int[] positions)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Position;
            var positionServiceUrl = misApiUrl + endPoint;

            var response = await Execute<MisResponse<PositionModel>>(positionServiceUrl, "positionId", positions);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByRoleId(int roleId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.Position;
            var positionServiceUrl = misApiUrl + endPoint + "?roleId=" + roleId;
            var response = await Execute<MisResponse<PositionModel>>(positionServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            return response.Data ?? new List<PositionModel>();
        }

        public async Task<IEnumerable<PersonPositionModel>> GetPersonPosition(int personId)
        {
            var misApiUrl = _misApiModel.CurrentValue.Url;
            var endPoint = _misApiModel.CurrentValue.Endpoint.PersonPosition;
            var personServiceUrl = misApiUrl + endPoint + "?personId=" + personId;
            var response = await Execute<MisResponse<PersonPositionModel>>(personServiceUrl);

            if (response == null)
            {
                RaiseNullResponseException();
                return null;
            }

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        private async Task<T> Execute<T>(string url)
        {
            var request = new RestRequest(url)
            {
                Method = Method.Get
            };
            request.AddHeader("Content-type", "application/json");
            foreach (var header in _misApiModel.CurrentValue.Headers)
                request.AddHeader(header.Key, header.Value);

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
                if (response.Content != null)
                    return JsonConvert.DeserializeObject<T>(response.Content);

            throw new ApplicationException(response.Content);
        }

        private async Task<T> Execute<T>(string url, string paramName, int[] paramValues)
        {
            var request = new RestRequest(url)
            {
                Method = Method.Get
            };
            request.AddHeader("Content-type", "application/json");
            foreach (var header in _misApiModel.CurrentValue.Headers)
                request.AddHeader(header.Key, header.Value);

            paramValues.ToList().ForEach(x => request.AddParameter(paramName, x, ParameterType.GetOrPost));

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
                if (response.Content != null)
                    return JsonConvert.DeserializeObject<T>(response.Content);

            throw new ApplicationException(response.Content);
        }

        private void RaiseApplicationException<T>(MisResponse<T> response)
        {
            var errorMessage = response.Errors?.FirstOrDefault()?.ErrorId + "-" +
                               response.Errors?.FirstOrDefault()?.StatusCode + "-" +
                               response.Errors?.FirstOrDefault()?.Message;
            _logger.LogErrorExtension(errorMessage, null);
            throw new ApplicationException(errorMessage);
        }

        private void RaiseNullResponseException()
        {
            const string errorMessage = "Received NULL response from MIS Api.";
            _logger.LogErrorExtension(errorMessage, null);
            throw new Exception(errorMessage);
        }
    }
}