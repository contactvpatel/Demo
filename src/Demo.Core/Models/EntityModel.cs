using Newtonsoft.Json;

namespace Demo.Core.Models
{
    public class EntityModel
    {
        [JsonProperty("entityId")] 
        public int EntityId { get; set; }

        [JsonProperty("entityUUID")] 
        public Guid EntityUuid { get; set; }

        [JsonProperty("divId")] 
        public int DivId { get; set; }

        [JsonProperty("code")] 
        public string Code { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("geoLevelId")] 
        public int GeoLevelId { get; set; }

        [JsonProperty("typeId")] 
        public int TypeId { get; set; }
    }
}
