using Demo.Api.Extensions;
using Demo.Api.Filters;
using Demo.Api.HealthCheck;
using Demo.Api.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.Data.SqlClient;
using ApiError = Demo.Api.Middleware.ApiError;

namespace Demo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // service dependencies         
            services.ConfigureDemoServices(Configuration);

            // Redis Cache for Distributed Caching Purpose
            services.ConfigureRedisCache(Configuration);

            // In-Memory Cache for scenario specific
            services.AddMemoryCache();

            services.ConfigureApiVersioning();

            services.ConfigureCors();

            services.ConfigureSwagger();

            services.AddControllers(options =>
                {
                    // Token Authorization
                    //options.Filters.Add(typeof(CustomAuthorization));

                    //options.ReturnHttpNotAcceptable = true;

                    //Filter to track Action Performance for Entire application's actions
                    options.Filters.Add(typeof(TrackActionPerformanceFilter));

                    options.Filters.Add<ValidationFilter>();
                })
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddFluentValidation(options => { options.RegisterValidatorsFromAssemblyContaining<Startup>(); })
                //.AddXmlDataContractSerializerFormatters()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Global Exception Handler Middleware
            app.UseApiExceptionHandler(options =>
            {
                options.AddResponseDetails = UpdateApiErrorResponse;
                options.DetermineLogLevel = DetermineLogLevel;
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1.0/swagger.json", "v1.0");
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
        }

        private static LogLevel DetermineLogLevel(Exception ex)
        {
            if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
            {
                return LogLevel.Critical;
            }

            return LogLevel.Error;
        }

        private void UpdateApiErrorResponse(HttpContext context, Exception ex, Models.Response<ApiError> apiError)
        {
            if (ex.GetType().Name == nameof(SqlException))
            {
                apiError.Message = "Exception was a database exception!";
            }
        }
    }
}