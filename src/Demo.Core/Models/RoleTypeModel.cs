using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class RoleTypeModel
    {
        [JsonProperty("roleTypeId")]
        public int RoleTypeId { get; set; }
        [JsonProperty("roleTypeUUID")]
        public Guid RoleTypeUuid { get; set; }
        [JsonProperty("name")]
        public string RoleTypeName { get; set; }
        [JsonProperty("code")]
        public string RoleTypeCode { get; set; }
        [JsonProperty("geoLevelId")]
        public int GeoLevelId { get; set; }
        [JsonProperty("geoLevel")]
        public string GeoLevelName { get; set; }
    }
}
