using Demo.Core.Models;
using Demo.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Business.Services
{
    public class MisService : Interfaces.IMisService
    {
        private readonly IMisService _misService;
        private readonly IMemoryCache _memoryCache;
        private readonly IRedisCacheService _redisCacheService;

        public MisService(IMisService misService, IMemoryCache memoryCache, IRedisCacheService redisCacheService)
        {
            _misService = misService ?? throw new ArgumentNullException(nameof(misService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<IEnumerable<DepartmentModel>> GetAllDepartments(int divisionId)
        {
            // In-Memory Cache Example
            const string cacheKey = "departments";

            var cachedResponse = _memoryCache.Get<IEnumerable<DepartmentModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse;

            var departments = (await _misService.GetAllDepartments(divisionId)).ToList();

            _memoryCache.Set(cacheKey, departments, TimeSpan.FromSeconds(86400));

            return departments.OrderBy(x => x.DepartmentName);
        }

        public async Task<DepartmentModel> GetDepartmentById(int id)
        {
            // In-Memory Cache Example
            const string cacheKey = "departments";

            var cachedResponse = _memoryCache.Get<IEnumerable<DepartmentModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse.FirstOrDefault(x => x.DepartmentId == id);

            var departments = (await _misService.GetAllDepartments(1)).ToList();

            _memoryCache.Set(cacheKey, departments, TimeSpan.FromSeconds(86400));

            return departments.FirstOrDefault(x => x.DepartmentId == id);
        }

        public async Task<IEnumerable<RoleTypeModel>> GetAllRoleTypes(int divisionId)
        {
            const string cacheKey = "role-types";

            var cachedResponse = await _redisCacheService.GetCachedData<IEnumerable<RoleTypeModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse;

            var roleTypes = (await _misService.GetAllRoleTypes(divisionId)).ToList();

            await _redisCacheService.SetCacheData(cacheKey, roleTypes, TimeSpan.FromSeconds(86400));

            return roleTypes.OrderBy(x => x.RoleTypeName);
        }

        public async Task<IEnumerable<RoleModel>> GetAllRoles(int divisionId)
        {
            const string cacheKey = "roles";

            var cachedResponse = await _redisCacheService.GetCachedData<IEnumerable<RoleModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse;

            var roles = (await _misService.GetAllRoles(divisionId)).ToList();

            await _redisCacheService.SetCacheData(cacheKey, roles, TimeSpan.FromSeconds(86400));

            return roles.OrderBy(x => x.RoleName);
        }

        public async Task<IEnumerable<RoleModel>> GetRoles(int divisionId, int[] roleIds)
        {
            const string cacheKey = "roles";

            var cachedResponse = await _redisCacheService.GetCachedData<IEnumerable<RoleModel>>(cacheKey);
            if (cachedResponse != null)
            {
                return cachedResponse.Where(r => roleIds.Contains(r.RoleId));
            }

            var roles = (await _misService.GetAllRoles(divisionId)).ToList();

            await _redisCacheService.SetCacheData(cacheKey, roles, TimeSpan.FromSeconds(86400));

            return roles.Where(r => roleIds.Contains(r.RoleId)).OrderBy(x => x.RoleName);
        }

        public async Task<RoleModel> GetRoleById(int id)
        {
            const string cacheKey = "roles";

            var cachedResponse = await _redisCacheService.GetCachedData<IEnumerable<RoleModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse.FirstOrDefault(x => x.RoleId == id);

            return await _misService.GetRoleById(id);
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByDepartmentId(int departmentId)
        {
            var cacheKey = $"roles-department-{departmentId}";

            var cachedResponse = await _redisCacheService.GetCachedData<IEnumerable<RoleModel>>(cacheKey);
            if (cachedResponse != null)
                return cachedResponse;

            var roles = (await _misService.GetRolesByDepartmentId(departmentId)).ToList();

            await _redisCacheService.SetCacheData(cacheKey, roles, TimeSpan.FromSeconds(86400));

            return roles.OrderBy(x => x.RoleName);
        }

        public async Task<IEnumerable<PositionModel>> GetPositions(int[] positionIds)
        {
            return await _misService.GetPositions(positionIds);
        }

        public async Task<IEnumerable<PositionModel>> GetPositionsByRoleId(int roleId)
        {
            var positions = (await _misService.GetPositionsByRoleId(roleId)).ToList();
            var entities = (await _misService.GetEntityByDivision(1)).ToList();
            foreach (var currentPosition in positions)
            {
                var selectedEntity = entities.FirstOrDefault(x => x.EntityId == currentPosition.EntityId);
                if (selectedEntity == null) continue;
                currentPosition.EntityName = selectedEntity.Name;
            }

            return positions.OrderBy(x => x.EntityName).ThenBy(x => x.PersonFirstName);
        }

        public async Task<IEnumerable<PersonPositionModel>> GetPersonPosition(int personId)
        {
            var personPositions = (await _misService.GetPersonPosition(personId)).ToList();
            var departments = (await _misService.GetAllDepartments(1)).ToList();
            foreach (var currentPersonPosition in personPositions)
            {
                foreach (var department in currentPersonPosition.Department)
                {
                    var selectedDepartment = departments.FirstOrDefault(x => x.DepartmentId == department.DepartmentId);
                    if (selectedDepartment == null) continue;
                    department.DepartmentUuid = selectedDepartment.DepartmentUuid;
                    department.DivisionId = selectedDepartment.DivisionId;
                    department.IsSatsangActivityDepartment = selectedDepartment.IsSatsangActivityDepartment;
                    department.IsAdministrationDepartment = selectedDepartment.IsAdministrationDepartment;
                    department.IsApplicationDepartment = selectedDepartment.IsApplicationDepartment;
                }
            }

            return personPositions.OrderBy(x => x.EntityName);
        }
    }
}
