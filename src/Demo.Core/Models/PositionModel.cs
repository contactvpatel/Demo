using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class PositionModel
    {
        [JsonProperty("positionId")]
        public int PositionId { get; set; }
        
        [JsonProperty("name")]
        public string PositionName { get; set; }
        
        [JsonProperty("shortName")]
        public string PositionShortName { get; set; }

        [JsonProperty("dept")]
        public IEnumerable<DepartmentModel> Department { get; set; }

        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("geoLevelId")]
        public int GeoLevelId { get; set; }

        [JsonProperty("entityId")]
        public int EntityId { get; set; }

        [JsonProperty("entityName")]
        public string EntityName { get; set; }

        [JsonProperty("occurance")]
        public int Occurance { get; set; }

        [JsonProperty("personId")]
        public int? PersonId { get; set; }

        [JsonProperty("personFName")]
        public string PersonFirstName { get; set; }

        [JsonProperty("personMName")]
        public string PersonMiddleName { get; set; }

        [JsonProperty("personLName")]
        public string PersonLastName { get; set; }

        [JsonProperty("personOName")]
        public string PersonOtherName { get; set; }
    }
}
