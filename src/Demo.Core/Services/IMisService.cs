using Demo.Core.Models;

namespace Demo.Core.Services
{
    public interface IMisService
    {
        Task<IEnumerable<DepartmentModel>> GetAllDepartments(int divisionId);
        Task<DepartmentModel> GetDepartmentById(int id);
        Task<IEnumerable<EntityModel>> GetEntityByDivision(int divisionId);
        Task<IEnumerable<RoleTypeModel>> GetAllRoleTypes(int divisionId);
        Task<IEnumerable<RoleModel>> GetAllRoles(int divisionId);
        Task<RoleModel> GetRoleById(int id);
        Task<IEnumerable<RoleModel>> GetRolesByDepartmentId(int departmentId);
        Task<IEnumerable<RoleModel>> GetRolesByDepartments(int[] departmentIds);
        Task<IEnumerable<PositionModel>> GetAllPositions(int divisionId);
        Task<IEnumerable<PositionModel>> GetPositions(int[] positions);
        Task<IEnumerable<PositionModel>> GetPositionsByRoleId(int roleId);
        Task<IEnumerable<PersonPositionModel>> GetPersonPosition(int personId);
    }
}
