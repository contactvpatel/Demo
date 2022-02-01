using Demo.Core.Models;

namespace Demo.Business.Interfaces
{
    public interface IMisService
    {
        Task<IEnumerable<DepartmentModel>> GetAllDepartments(int divisionId);
        Task<DepartmentModel> GetDepartmentById(int id);
        Task<IEnumerable<RoleTypeModel>> GetAllRoleTypes(int divisionId);
        Task<IEnumerable<RoleModel>> GetAllRoles(int divisionId);
        Task<IEnumerable<RoleModel>> GetRoles(int divisionId, int[] roleIds);
        Task<RoleModel> GetRoleById(int id);
        Task<IEnumerable<RoleModel>> GetRolesByDepartmentId(int departmentId);
        Task<IEnumerable<PositionModel>> GetPositions(int[] positionIds);
        Task<IEnumerable<PositionModel>> GetPositionsByRoleId(int roleId);
        Task<IEnumerable<PersonPositionModel>> GetPersonPosition(int personId);
    }
}
