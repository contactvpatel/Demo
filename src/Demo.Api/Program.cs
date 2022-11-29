using Demo.Api.Extensions;
using Demo.Api.Filters;
using Demo.Api.HealthCheck;
using Demo.Api.Middleware;
using Demo.Api.Models;
using Demo.Util.Models;
using FluentValidation.AspNetCore;
using Serilog;
using System.Data.SqlClient;
using System.Diagnostics;
using Demo.Util.ApplicationSettingConfiguration;
using Demo.Util.Logging;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Cloud Deployment uses secret file specified inside AddApplicationSetting.
        // For Local Development, AddApplicationSetting's Optional property should be true.
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddApplicationSetting("/vault/secrets/", optional: Debugger.IsAttached, reloadOnChange: true)
                            .AddEnvironmentVariables();

        builder.Host.UseSerilog((context, provider, loggerConfig) =>
        {
            loggerConfig.LogConfiguration(context, provider, builder.Configuration);
        });

        // service dependencies         
        builder.Services.ConfigureDemoServices(builder.Configuration);

        // Redis Cache for Distributed Caching Purpose
        builder.Services.ConfigureRedisCache(builder.Configuration);

        // In-Memory Cache for scenario specific
        builder.Services.AddMemoryCache();

        builder.Services.ConfigureApiVersioning();

        builder.Services.ConfigureCors();

        builder.Services.ConfigureSwagger();

        var appSettings = new AppSettingModel();
        builder.Configuration.GetSection("AppSettings").Bind(appSettings);

        builder.Services.AddControllers(options =>
            {
                // SSO Token Authorization
                //options.Filters.Add(typeof(CustomAuthorization));

                //Filter to track Action Performance for Entire application's actions
                if (appSettings.EnablePerformanceFilterLogging)
                {
                    options.Filters.Add(typeof(TrackActionPerformanceFilter));
                }

                options.Filters.Add<ValidationFilter>();
            })
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
            .AddFluentValidation(options => { options.RegisterValidatorsFromAssemblyContaining<Program>(); });

        var app = builder.Build();

        // Global Exception Handler Middleware
        app.UseApiExceptionHandler(options =>
        {
            options.AddResponseDetails = UpdateApiErrorResponse;
            options.DetermineLogLevel = DetermineLogLevel;
        });

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "v1");
            //options.SwaggerEndpoint("/v2/swagger.json", "v2"); // Future Version
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultHealthChecks();
            endpoints.MapControllers();
        });

        app.Run();

        static LogLevel DetermineLogLevel(Exception ex)
        {
            if (ex.Message.StartsWith("error occurred using the connection to database",
                    StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
            {
                return LogLevel.Critical;
            }

            return LogLevel.Error;
        }

        static void UpdateApiErrorResponse(HttpContext context, Exception ex, Response<ApiError> apiError)
        {
            if (ex.GetType().Name == nameof(SqlException))
            {
                apiError.Message = "Exception was a database exception!";
            }
        }
    }
}