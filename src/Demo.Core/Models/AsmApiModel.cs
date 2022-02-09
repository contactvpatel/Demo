namespace Demo.Core.Models
{
    public class AsmApiModel
    {
        public string Url { get; set; }
        public AsmEndpoint Endpoint { get; set; }
    }

    public class AsmEndpoint
    {
        public string ApplicationSecurity { get; set; }
    }

    public class AsmResponse<T>
    {
        public bool Succeeded { get; set; }
        public List<T> Data { get; set; }
        public string Message { get; set; }
        public List<ApiError> Errors { get; set; }
    }

    public class ApiError
    {
        public string ErrorId { get; set; }
        public short StatusCode { get; set; }
        public string Message { get; set; }
    }
}