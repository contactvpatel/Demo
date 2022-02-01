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
    public class MisService : IMisService
    {
        private readonly IRestClient _client;
        private readonly IRestRequest _request;
        private readonly IOptions<MisApiModel> _misApiModel;
        private readonly ILogger<MisService> _logger;

        public MisService(IRestClient client, IOptions<MisApiModel> misApiModel, ILogger<MisService> logger)
        {
            _misApiModel = misApiModel ?? throw new ArgumentNullException(nameof(misApiModel));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _request = new RestRequest();
        }

        public async Task<IEnumerable<DepartmentModel>> GetAllDepartments(int divisionId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Department;
            var departmentServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<DepartmentModel>>(departmentServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<DepartmentModel> GetDepartmentById(int id)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Department;
            var departmentServiceUrl = misApiUrl + endPoint + "?deptId=" + id;
            var response = await Execute<MisResponse<DepartmentModel>>(departmentServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data.FirstOrDefault();
        }
      
        public async Task<IEnumerable<EntityModel>> GetEntityByDivision(int divisionId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Entity;
            var entityServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<EntityModel>>(entityServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleTypeModel>> GetAllRoleTypes(int divisionId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.RoleType;
            var roleTypeServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<RoleTypeModel>>(roleTypeServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleModel>> GetAllRoles(int divisionId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?divId=" + divisionId;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<RoleModel> GetRoleById(int id)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?roleId=" + id;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByDepartmentId(int departmentId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint + "?deptId=" + departmentId;
            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByDepartments(int[] departmentIds)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Role;
            var roleServiceUrl = misApiUrl + endPoint;

            var response = await Execute<MisResponse<RoleModel>>(roleServiceUrl, "deptId", departmentIds);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<PositionModel>> GetPositions(int[] positions)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Position;
            var roleServiceUrl = misApiUrl + endPoint;

            var response = await Execute<MisResponse<PositionModel>>(roleServiceUrl, "positionId", positions);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByRoleId(int roleId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.Position;
            var positionServiceUrl = misApiUrl + endPoint + "?roleId=" + roleId;
            var response = await Execute<MisResponse<PositionModel>>(positionServiceUrl);
            return response.Data ?? new List<PositionModel>();
        }

        public async Task<IEnumerable<PersonPositionModel>> GetPersonPosition(int personId)
        {
            var misApiUrl = _misApiModel.Value.Url;
            var endPoint = _misApiModel.Value.Endpoint.PersonPosition;
            var personServiceUrl = misApiUrl + endPoint + "?personId=" + personId;
            var response = await Execute<MisResponse<PersonPositionModel>>(personServiceUrl);

            if (!response.Succeeded)
                RaiseApplicationException(response);

            return response.Data;
        }

        private async Task<T> Execute<T>(string url)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = Method.GET;
            _request.AddHeader("Content-type", "application/json");
            foreach (var header in _misApiModel.Value.Headers)
                _request.AddHeader(header.Key, header.Value);

            var response = await _client.ExecuteAsync(_request);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
                return JsonConvert.DeserializeObject<T>(response.Content);

            throw new ApplicationException(response.Content);
        }

        private async Task<T> Execute<T>(string url, string paramName, int[] paramValues)
        {
            _request.Parameters.Clear();
            _request.Resource = url;
            _request.Method = Method.GET;
            _request.AddHeader("Content-type", "application/json");
            foreach (var header in _misApiModel.Value.Headers)
                _request.AddHeader(header.Key, header.Value);

            paramValues.ToList().ForEach(x => _request.AddParameter(paramName, x, ParameterType.GetOrPost));

            var response = await _client.ExecuteAsync(_request);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
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
    }
}
