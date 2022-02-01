using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class PersonPositionModel
    {
        [JsonProperty("positionId")]
        public int PositionId { get; set; }

        [JsonProperty("positionUUID")]
        public Guid PositionUuid { get; set; }

        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        [JsonProperty("roleUUID")]
        public Guid RoleUuid { get; set; }

        [JsonProperty("name")]
        public string RoleName { get; set; }

        [JsonProperty("shortName")]
        public string RoleShortName { get; set; }

        [JsonProperty("roleType")]
        public string RoleTypeCode { get; set; }

        [JsonProperty("dept")]
        public IEnumerable<DepartmentModel> Department { get; set; }

        [JsonProperty("entityId")]
        public int EntityId { get; set; }

        [JsonProperty("entityName")]
        public string EntityName { get; set; }

        [JsonProperty("geoLevelId")]
        public int GeoLevelId { get; set; }

        [JsonProperty("personId")]
        public int PersonId { get; set; }

        [JsonProperty("personUUID")]
        public Guid PersonUuid { get; set; }

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