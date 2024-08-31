using Demo.Business.Models;
using Demo.Core.Models;
using Demo.Util.FIQL;
using Demo.Util.Models;
using System.Text.Json;

namespace Demo.Api.Middleware
{
    public class QueryTrackerMiddleware
    {
        private readonly RequestDelegate _next;

        public QueryTrackerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, QueryTrackerService queryTracker)
        {
            // Capture the original response body
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Call the next middleware in the pipeline
                await _next(context);

                // If the response is not an error
                if (context.Response.StatusCode == StatusCodes.Status200OK && context.Response.ContentType?.Contains("application/json") == true)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // Deserialize the original response body
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true  // Case-sensitive
                    };
                    var originalResponse = JsonSerializer.Deserialize<HttpResponseModel>(responseBodyContent, options);

                    var includeSqlQueryCount = context.Request.Query.Any(x => x.Key.ToLower() == "include" && x.Value.ToString().ToLower().Contains("sqlquerycount"));
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
                        var modifiedResponseBody = JsonSerializer.Serialize(modifiedResponse);
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
                        var modifiedResponseBody = JsonSerializer.Serialize(modifiedResponse);
                        await context.Response.WriteAsync(modifiedResponseBody);
                    };
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    // Copy the contents of the new response body to the original response stream
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }

}