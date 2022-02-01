using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class RoleModel
    {
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        
        [JsonProperty("roleUUID")]
        public Guid RoleUuid { get; set; }
        
        [JsonProperty("name")]
        public string RoleName { get; set; }
        
        [JsonProperty("shortName")]
        public string RoleShortName { get; set; }
        
        [JsonProperty("divId")]
        public int DivisionId { get; set; }
        
        [JsonProperty("geoLevelId")]
        public int GeoLevelId { get; set; }
        
        [JsonProperty("dept")]
        public IEnumerable<DepartmentModel> Departments { get; set; }
    }
}
