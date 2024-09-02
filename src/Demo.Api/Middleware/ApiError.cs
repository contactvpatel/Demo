namespace Demo.Api.Middleware
{
    public class ApiError
    {
        public string ErrorId { get; set; }

        public short StatusCode { get; set; }

        public string Message { get; set; }
        public string DevEnvErrorDetails { get; set; }
    }
}
