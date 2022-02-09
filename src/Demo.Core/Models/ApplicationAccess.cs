namespace Demo.Core.Models
{
    public class ApplicationAccess
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public int ModuleTypeId { get; set; }
        public string ModuleType { get; set; }
        public bool? HasViewAccess { get; set; }
        public bool? HasCreateAccess { get; set; }
        public bool? HasUpdateAccess { get; set; }
        public bool? HasDeleteAccess { get; set; }
        public bool? HasAccess { get; set; }
    }
}
