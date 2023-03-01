using Demo.Api.Models;
using Demo.Core.Models;
using Demo.Util.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    /// <summary>
    /// MIS Controller. 
    /// </summary>
    [Route("/mis")]
    [ApiController]
    [ApiVersion("1")]
    public class MisController : ControllerBase
    {
        private readonly Business.Interfaces.IMisService _misService;
        private readonly ILogger<MisController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="misService">MIS Service</param>
        /// <param name="logger">Logger</param>
        public MisController(Business.Interfaces.IMisService misService, ILogger<MisController> logger)
        {
            _misService = misService ?? throw new ArgumentNullException(nameof(misService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Return Departments
        /// </summary>  
        [HttpGet("departments", Name = "GetAllDepartments")]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAllDepartments(int divisionId)
        {
            _logger.LogInformationExtension("Get All Departments");

            var departments = (await _misService.GetAllDepartments(divisionId)).ToList();

            var message = !departments.Any() ? "No departments found" : $"Found {departments.Count} departments";

            _logger.LogInformationExtension(message);

            return Ok(new Response<IEnumerable<DepartmentModel>>(departments, message));
        }

        /// <summary>
        /// Return Department By Id.
        /// </summary>
        /// <param name="id">Department Id.</param>
        [HttpGet("departments/{id:int}", Name = "GetDepartmentById")]
        public async Task<ActionResult<DepartmentModel>> GetDepartmentById(int id)
        {
            _logger.LogInformationExtension($"Get Department By Id: {id}");
            var department = await _misService.GetDepartmentById(id);
            if (department == null)
            {
                var message = $"No department found with id {id}";
                _logger.LogInformationExtension(message);
                return Ok(new Response<DepartmentModel>(null, message));
            }

            return Ok(new Response<DepartmentModel>(department));
        }

        /// <summary>
        /// Return Role Types
        /// </summary>  
        [HttpGet("role-types", Name = "GetAllRoleTypes")]
        public async Task<ActionResult<IEnumerable<RoleTypeModel>>> GetAllRoleTypes(int divisionId)
        {
            _logger.LogInformationExtension("Get All Role Types");

            var roleTypes = (await _misService.GetAllRoleTypes(divisionId)).ToList();

            var message = !roleTypes.Any() ? "No role types found" : $"Found {roleTypes.Count} role types";

            _logger.LogInformationExtension(message);

            return Ok(new Response<IEnumerable<RoleTypeModel>>(roleTypes, message));
        }

        /// <summary>
        /// Return Roles
        /// </summary>  
        [HttpGet("roles", Name = "GetAllRoles")]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetAllRoles(int divisionId)
        {
            _logger.LogInformationExtension("Get All Roles");

            var roles = (await _misService.GetAllRoles(divisionId)).ToList();

            var message = !roles.Any() ? "No roles found" : $"Found {roles.Count} roles";

            _logger.LogInformationExtension(message);

            return Ok(new Response<IEnumerable<RoleModel>>(roles, message));
        }

        /// <summary>
        /// Return Role by Id
        /// </summary>  
        [HttpGet("roles/{id:int}", Name = "GetRoleById")]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetRoleById(int id)
        {
            _logger.LogInformationExtension($"Get Role By Id: {id}");

            var role = await _misService.GetRoleById(id);

            var message = role == null ? $"No role found with id {id}" : string.Empty;

            _logger.LogInformationExtension(message);

            return Ok(new Response<RoleModel>(role));
        }

        /// <summary>
        /// Return Roles by Department Id
        /// </summary>  
        [HttpGet("roles/departments/{departmentId:int}", Name = "GetRolesByDepartmentId")]
        public async Task<ActionResult<IEnumerable<RoleModel>>> GetRolesByDepartmentId(int departmentId)
        {
            _logger.LogInformationExtension($"Get Role By Department Id: {departmentId}");

            var roles = (await _misService.GetRolesByDepartmentId(departmentId)).ToList();

            var message = !roles.Any()
                ? $"No roles found for department id: {departmentId}"
                : $"Found {roles.Count} roles for department id: {departmentId}";

            _logger.LogInformationExtension(message);

            return Ok(new Response<IEnumerable<RoleModel>>(roles, message));
        }

        /// <summary>
        /// Return Positions by Role Id
        /// </summary>  
        [HttpGet("positions/roles/{roleId:int}", Name = "GetPositionByRoleId")]
        public async Task<ActionResult<IEnumerable<PositionModel>>> GetPositionByRoleId(int roleId)
        {
            _logger.LogInformationExtension($"Get Position By Role Id: {roleId}");

            var positions = (await _misService.GetPositionsByRoleId(roleId)).ToList();

            var message = !positions.Any()
                ? $"No positions found for role id: {roleId}"
                : $"Found {positions.Count} positions for role id: {roleId}";

            _logger.LogInformationExtension(message);

            return Ok(new Response<IEnumerable<PositionModel>>(positions, message));
        }

        [HttpGet("person-positions/{id:int}", Name = "GetPersonPosition")]
        public async Task<IActionResult> GetPersonPosition(int id)
        {
            _logger.LogInformationExtension($"Get Permissions by Person Id: {id}");

            var permissions = await _misService.GetPersonPosition(id);

            return Ok(new Response<IEnumerable<PersonPositionModel>>(permissions));
        }
    }
}
