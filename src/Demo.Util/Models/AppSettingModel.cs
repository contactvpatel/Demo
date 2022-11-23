namespace Demo.Util.Models
{
    public class AppSettingModel
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationSecret { get; set; }
        public bool EnableAsmAuthorization { get; set; }
        public bool EnablePerformanceFilterLogging { get; set; }
    }
}
