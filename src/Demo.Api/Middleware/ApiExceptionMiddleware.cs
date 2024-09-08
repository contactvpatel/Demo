using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Demo.Api.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;
        private readonly ApiExceptionOptions _options;

        public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next,
            ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorId = !string.IsNullOrEmpty(exception.Data["ErrorId"]?.ToString())
                ? exception.Data["ErrorId"]?.ToString()
                : Guid.NewGuid().ToString();

            var apiErrors = new List<ApiError>
            {
                new()
                {
                    ErrorId = errorId,
                    StatusCode = exception.GetType() == typeof(UnauthorizedAccessException)
                        ? (short)HttpStatusCode.Forbidden
                        : (short)HttpStatusCode.InternalServerError,
                    Message = exception.GetType() == typeof(ApplicationException) ||
                              exception.GetType() == typeof(UnauthorizedAccessException)
                        ? exception.Message
                        : $"Error occurred in the API. Please use the ErrorId [{errorId}] and contact support team if the problem persists."
                }
            };

            var errorResponse = new Models.Response<ApiError>(null) { Errors = apiErrors };

            _options.AddResponseDetails?.Invoke(context, exception, errorResponse);

            var innerExMessage = GetInnermostExceptionMessage(exception);

            var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;

            if (string.IsNullOrEmpty(exception.Data["ErrorId"]?.ToString()))
                exception.Data.Add("ErrorId", errorId);

            _logger.Log(level, exception, $"Exception Occurred: {innerExMessage} -- ErrorId: {errorId}");

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var result = JsonConvert.SerializeObject(errorResponse, serializerSettings);
            context.Response.StatusCode = exception.GetType() == typeof(UnauthorizedAccessException)
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(result);
        }

        private static string GetInnermostExceptionMessage(Exception exception)
        {
            while (true)
            {
                if (exception.InnerException == null)
                    return exception.Message;
                exception = exception.InnerException;
            }
        }
    }
}
