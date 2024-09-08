using Demo.Util.FIQL;
using Demo.Util.Models;
using System.Data;

namespace Demo.Api.Middleware
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, QueryTrackerService queryTracker)
        {
            // Exclude Swagger or Non GET requests (POST, PATCH, PUT, DELETE)
            if (context.Request.Path.StartsWithSegments("/swagger") || !context.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Backup the original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                using var newBodyStream = new MemoryStream();

                context.Response.Body = newBodyStream;

                // Continue processing the request
                await _next(context);

                // Reset the body stream position to the beginning
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Read the response body
                var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

                // Parse the response body to JSON, add a timestamp, and modify the response
                if (context.Response.StatusCode == StatusCodes.Status200OK && context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
                {
                    // Deserialize the original response body
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true  // Case-sensitive
                    };

                    var originalResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseModel>(responseBody, options);
                    var includeSqlQueryCount = context.Request.Query.Any(x => x.Key.Equals("include", StringComparison.CurrentCultureIgnoreCase) && x.Value.ToString().Contains("sqlquerycount", StringComparison.CurrentCultureIgnoreCase));

                    if (includeSqlQueryCount)
                    {
                        var modifiedResponse = new
                        {
                            originalResponse.Status,
                            originalResponse.Message,
                            originalResponse.TotalRecords,
                            SqlQueryCount = queryTracker.QueryCount,
                            originalResponse.Data
                        };
                        // Serialize the modified response and write it back to the response body
                        var modifiedResponseBody = System.Text.Json.JsonSerializer.Serialize(modifiedResponse);

                        // Write the modified body to the original response stream
                        context.Response.Body = originalBodyStream;
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

                        // Write the modified body to the original response stream
                        context.Response.Body = originalBodyStream;
                        await context.Response.WriteAsync(modifiedResponseBody);
                    };
                }
                else
                {
                    // For non-JSON responses, return the original response
                    context.Response.Body = originalBodyStream;
                    await context.Response.WriteAsync(responseBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while generating a response: {ex.Message}");
                context.Response.Body = originalBodyStream;
                throw;
            }
        }
    }
}