namespace Demo.Api.Middleware
{
    public static class ApiExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder,
            Action<ApiExceptionOptions> configureOptions)
        {
            var options = new ApiExceptionOptions();
            configureOptions(options);

            return builder.UseMiddleware<ApiExceptionMiddleware>(options);
        }
    }
}
