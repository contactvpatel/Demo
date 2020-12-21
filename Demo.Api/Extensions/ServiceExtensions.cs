using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Demo.Api.Extensions.Swagger;
using Demo.Application.Interfaces;
using Demo.Application.Services;
using Demo.Common.Logging;
using Demo.Core.Configuration;
using Demo.Core.Repositories;
using Demo.Core.Repositories.Base;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories;
using Demo.Infrastructure.Repositories.Base;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDemoServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Core Layer
            services.Configure<DemoSettings>(configuration);

            // Add Infrastructure Layer
            ConfigureDatabases(services);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Add Application Layer
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // LoggingHelpers
            services.AddTransient<LoggingDelegatingHandler>();

            // HealthChecks
            services.AddHealthChecks().AddDbContextCheck<DemoContext>();
        }

        public static void ConfigureDatabases(IServiceCollection services)
        {
            // use in-memory database
            services.AddDbContext<DemoContext>(c =>
                c.UseInMemoryDatabase("DemoDbConnection"));

            //// use real database
            //services.AddDbContext<DemoContext>(c =>
            //    c.UseSqlServer(Configuration.GetConnectionString("DemoDbConnection")));
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                // Supporting multiple versioning scheme
                // Route (api/v1/accounts)
                // Header (X-version=1)
                // Querystring (api/accounts?api-version=1)
                // Media Type (application/json;v=1)
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-version"), new QueryStringApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader("v"));
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(ConfigureSwaggerGen);
        }

        private static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            AddSwaggerDocs(options);

            options.OperationFilter<RemoveVersionFromParameter>();
            options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

            options.DocInclusionPredicate((version, desc) =>
            {
                if (!desc.TryGetMethodInfo(out var methodInfo))
                    return false;

                var versions = methodInfo
                    .DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);

                var maps = methodInfo
                    .GetCustomAttributes(true)
                    .OfType<MapToApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions)
                    .ToList();

                return versions?.Any(v => $"v{v}" == version) == true
                       && (!maps.Any() || maps.Any(v => $"v{v}" == version));
            });
        }

        private static void AddSwaggerDocs(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1.0", new OpenApiInfo
            {
                Version = "v1.0",
                Title = "Demo API"
            });

            // Future Version
            //options.SwaggerDoc("v2", new OpenApiInfo
            //{
            //    Version = "v2",
            //    Detail = "Demo API"
            //});
        }
    }
}