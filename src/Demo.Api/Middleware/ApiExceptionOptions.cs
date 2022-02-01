namespace Demo.Api.Middleware
{
    public class ApiExceptionOptions
    {
        public Action<HttpContext, Exception, Models.Response<ApiError>> AddResponseDetails { get; set; }

        public Func<Exception, LogLevel> DetermineLogLevel { get; set; }
    }
}
