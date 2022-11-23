using System.ComponentModel;
using System.Net;
using System.Text;
using Demo.Api.Extensions;
using Demo.Business.Interfaces;
using Demo.Core.Models;
using Demo.Util.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Demo.Api.Filters
{
    public class AsmAuthorization : TypeFilterAttribute
    {
        public AsmAuthorization(ModuleCode moduleCode, AccessType accessType) : base(typeof(AccessAuthorization))
        {
            Arguments = new object[] { moduleCode, accessType };
        }
    }

    public class AccessAuthorization : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMisService _misService;
        private readonly IAsmService _asmService;
        private ModuleCode ModuleCode { get; }
        private AccessType AccessType { get; }

        public AccessAuthorization(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
            IMisService misService, IAsmService asmService,
            ModuleCode moduleCode, AccessType accessType)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _misService = misService ?? throw new ArgumentNullException(nameof(misService));
            _asmService = asmService ?? throw new ArgumentNullException(nameof(asmService));
            ModuleCode = moduleCode;
            AccessType = accessType;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var appSettings = new AppSettingModel();
            _configuration.GetSection("AppSettings").Bind(appSettings);

            if (!appSettings.EnableAsmAuthorization) return;

            // User Id will be exist once SSO Validation is successfully completed using CustomAuthorization filter
            var userId = _httpContextAccessor?.HttpContext?.Request.HttpContext.Items["UserId"]?.ToString();

            // Below list of headers must be passed to each API Call to perform authorization
            var applicationId = _httpContextAccessor?.HttpContext?.Request.Headers["x-baps-auth-app-id"].ToString();
            var applicationSecret =
                _httpContextAccessor?.HttpContext?.Request.Headers["x-baps-auth-app-secret"].ToString();
            var personId = _httpContextAccessor?.HttpContext?.Request.Headers["x-baps-auth-user-id"].ToString();
            var roleId = _httpContextAccessor?.HttpContext?.Request.Headers["x-baps-auth-role-id"].ToString();
            var positionId = _httpContextAccessor?.HttpContext?.Request.Headers["x-baps-auth-position-id"].ToString();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(personId) || string.IsNullOrEmpty(roleId) ||
                string.IsNullOrEmpty(positionId))
            {
                var response = "";

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(personId))
                    response = GetResponse("Unauthorized Access. User Id is missing.");
                else if (string.IsNullOrEmpty(roleId))
                    response = GetResponse("Unauthorized Access. Role Id is missing.");
                else if (string.IsNullOrEmpty(positionId))
                    response = GetResponse("Unauthorized Access. Position Id is missing.");
                else if (userId != personId) // Compare SSO User Id (Person Id) to User Id passed in header
                    response = GetResponse("Unauthorized Access. User Id is mismatched.");

                /* We may utilize Application Id & Secret in future */
                /*
                else if (appSettings.ApplicationId != applicationId)
                    response = GetResponse("Unauthorized Access. Application Id is mismatched.");
                else if (appSettings.ApplicationSecret != applicationSecret)
                    response = GetResponse("Unauthorized Access. Application Secret is mismatched.");
                */

                var byteArray = Encoding.ASCII.GetBytes(response);
                using var memoryStream = new MemoryStream(byteArray);

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.Body = memoryStream;
                context.Result = new UnauthorizedResult();
            }
            else
            {
                // TODO: Store PersonPosition in a cache, so doesn't need to hit MIS Api for each call
                var positions = _misService.GetPersonPosition(int.Parse(userId)).Result.ToList();
                if (positions.Count == 0)
                {
                    var byteArray =
                        Encoding.ASCII.GetBytes(
                            GetResponse("Unauthorized Access. User doesn't have any position assigned."));
                    using var memoryStream = new MemoryStream(byteArray);

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.Body = memoryStream;
                    context.Result = new UnauthorizedResult();
                }

                // Allow For Non-Protected (Read Only) Access
                if (AccessType == AccessType.Read)
                    return;

                var selectedPosition = positions.FirstOrDefault(x => x.PositionId == int.Parse(positionId));
                if (selectedPosition == null)
                {
                    var byteArray =
                        Encoding.ASCII.GetBytes(
                            GetResponse("Unauthorized Access. Position is mismatched."));
                    using var memoryStream = new MemoryStream(byteArray);

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.Body = memoryStream;
                    context.Result = new UnauthorizedResult();
                }

                // TODO: Store ASM Access Data in a cache, so doesn't need to hit ASM Api for each call
                var applicationSecurityRequestModel = new ApplicationSecurityRequestModel
                {
                    ApplicationId = Guid.Parse(appSettings.ApplicationId),
                    PersonId = int.Parse(userId),
                    Positions = positions.Select(currentPosition => new PositionRequestModel
                        { RoleId = currentPosition.RoleId, PositionId = currentPosition.PositionId }).ToList()
                };

                var accessPermissions = _asmService.Get(applicationSecurityRequestModel).Result.ToList();
                if (accessPermissions.Count == 0)
                {
                    var byteArray =
                        Encoding.ASCII.GetBytes(
                            GetResponse("Unauthorized Access. User doesn't have any permission assigned."));
                    using var memoryStream = new MemoryStream(byteArray);

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.Body = memoryStream;
                    context.Result = new UnauthorizedResult();
                }

                var selectedAccessPermission = accessPermissions.FirstOrDefault(x =>
                    x.RoleId == selectedPosition?.RoleId && x.PositionId == selectedPosition.PositionId);

                if (selectedAccessPermission == null)
                {
                    var byteArray = Encoding.ASCII.GetBytes(GetResponse(
                        "Unauthorized Access. User doesn't have any permission assigned for selected role & position."));
                    using var memoryStream = new MemoryStream(byteArray);

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.Body = memoryStream;
                    context.Result = new UnauthorizedResult();
                }

                if (HasAccess(ModuleCode, AccessType, selectedAccessPermission?.ApplicationAccess)) return;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new UnauthorizedResult();
            }
        }

        private static bool HasAccess(ModuleCode moduleCode, AccessType accessType,
            IEnumerable<ApplicationAccess> accessPermissions)
        {
            if (accessPermissions == null)
                return false;

            switch (accessType)
            {
                case AccessType.View:
                    if (accessPermissions.Any(r =>
                            r.HasViewAccess == true && r.ModuleCode == moduleCode.GetDescription()))
                        return true;
                    break;
                case AccessType.Create:
                    if (accessPermissions.Any(r =>
                            r.HasCreateAccess == true && r.ModuleCode == moduleCode.GetDescription()))
                        return true;
                    break;
                case AccessType.Update:
                    if (accessPermissions.Any(r =>
                            r.HasUpdateAccess == true && r.ModuleCode == moduleCode.GetDescription()))
                        return true;
                    break;
                case AccessType.Delete:
                    if (accessPermissions.Any(r =>
                            r.HasDeleteAccess == true && r.ModuleCode == moduleCode.GetDescription()))
                        return true;
                    break;
                case AccessType.Access:
                    if (accessPermissions.Any(r =>
                            r.HasAccess == true && r.ModuleCode == moduleCode.GetDescription()))
                        return true;
                    break;
            }

            return false;
        }

        private static string GetResponse(string message)
        {
            var apiErrors = new List<Middleware.ApiError>
            {
                new()
                {
                    ErrorId = Guid.NewGuid().ToString(),
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Message = message
                }
            };

            var response = new Models.Response<ApiError>(null) { Errors = apiErrors };

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    public enum ModuleCode
    {
        [Description("PROD")] Product,
        [Description("CATE")] Category,
    }

    public enum AccessType
    {
        View,
        Create,
        Update,
        Delete,
        Access,
        Read
    }
}
