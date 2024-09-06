using System.Data;
using System.Net;
using Demo.Util.FIQL;
using Demo.Util.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Demo.Api.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;
        private readonly ApiExceptionOptions _options;
        private readonly IWebHostEnvironment _env;
        public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next,
            ILogger<ApiExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _options = options;
            _env = env;
        }

        public async Task Invoke(HttpContext context, IDbConnection dbConnection)
        {
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);

                    // If the response is not an error
                    if (context.Response.StatusCode == StatusCodes.Status200OK && context.Response.ContentType?.Contains("application/json") == true)
                    {
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);

                        // Deserialize the original response body
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true  // Case-sensitive
                        };
                        var originalResponse = System.Text.Json.JsonSerializer.Deserialize<HttpResponseModel>(responseBodyContent, options);

                        var includeSqlQueryCount = context.Request.Query.Any(x => x.Key.ToLower() == "include" && x.Value.ToString().ToLower().Contains("sqlquerycount"));
                        if (includeSqlQueryCount)
                        {
                            int queryCount = 0;
                            if (dbConnection is DbConnectionInterceptors countingConnection)
                                queryCount = countingConnection.GetQueryCount();

                            var modifiedResponse = new
                            {
                                originalResponse.Status,
                                originalResponse.Message,
                                originalResponse.TotalRecords,
                                SqlQueryCount = queryCount,
                                originalResponse.Data
                            };
                            // Serialize the modified response and write it back to the response body
                            var modifiedResponseBody = System.Text.Json.JsonSerializer.Serialize(modifiedResponse);
                            await context.Response.WriteAsync(modifiedResponseBody);
                        }
                        else
                        {
                            var modifiedResponse = new
                            {
                                originalResponse.Status,
                                originalResponse.TotalRecords,
                                originalResponse.Message,
                                originalResponse.Data
                            };
                            // Serialize the modified response and write it back to the response body
                            var modifiedResponseBody = System.Text.Json.JsonSerializer.Serialize(modifiedResponse);
                            await context.Response.WriteAsync(modifiedResponseBody);
                        };
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        // Copy the contents of the new response body to the original response stream
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex, responseBody, originalBodyStream);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, MemoryStream responseBody, Stream originalBodyStream)
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
                        : $"Error occurred in the API. Please use the ErrorId [{errorId}] and contact support team if the problem persists.",
                        DevEnvErrorDetails = _env.IsDevelopment() ? exception?.ToString() : null

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

            await context.Response.WriteAsync(result);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

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
