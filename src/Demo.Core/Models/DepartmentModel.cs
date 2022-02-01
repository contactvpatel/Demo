using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class DepartmentModel
    {
        [JsonProperty("deptId")]
        public int DepartmentId { get; set; }
        
        [JsonProperty("deptUUID")]
        public Guid DepartmentUuid { get; set; }
        
        [JsonProperty("name")]
        public string DepartmentName { get; set; }
        
        [JsonProperty("code")]
        public string DepartmentCode { get; set; }
        
        [JsonProperty("divId")]
        public int DivisionId { get; set; }
        
        [JsonProperty("wing")]
        public string Wing { get; set; }
        
        [JsonProperty("isSatsangActivityDept")]
        public bool IsSatsangActivityDepartment { get; set; }
        
        [JsonProperty("isApplicationsDept")]
        public bool IsApplicationDepartment { get; set; }
        
        [JsonProperty("isAdministrationDept")]
        public bool IsAdministrationDepartment { get; set; }
    }
}
