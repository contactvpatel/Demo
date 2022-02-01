using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Demo.Api.Extensions.Swagger;
using Demo.Core.Models;
using Demo.Infrastructure.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using RestSharp;
using Demo.Util.Logging;
using Microsoft.Data.SqlClient;
using Demo.Infrastructure.Repositories.Base;
using Demo.Core.Repositories.Base;
using Demo.Infrastructure.Repositories;
using Demo.Core.Repositories;
using Demo.Business.Services;
using Demo.Business.Interfaces;

namespace Demo.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDemoServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database
            ConfigureDatabases(services, configuration);

            // Using Scrutor to map the dependencies with scoped lifetime (https://github.com/khellang/Scrutor)
            //services.Scan(scan => scan
            //.FromCallingAssembly()
            //.FromApplicationDependencies(c => c.FullName != null && c.FullName.StartsWith("Demo"))
            //.AddClasses()
            //.AsMatchingInterface().WithScopedLifetime());

            // Add Infrastructure Layer
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<Core.Services.IMisService, Infrastructure.Services.MisService>();
            services.AddScoped<Core.Services.ISsoService, Infrastructure.Services.SsoService>();

            // Add Business Layer
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMisService, MisService>();
            services.AddScoped<ISsoService, SsoService>();

            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // LoggingHelpers
            services.AddTransient<LoggingDelegatingHandler>();

            //External Service Dependency (Example: MisService, SsoService)
            services.AddTransient<IRestClient>(_ => new RestClient(configuration.GetSection("MisService:Url").Value));
            services.Configure<MisApiModel>(configuration.GetSection("MisService"));
            services.Configure<SsoApiModel>(configuration.GetSection("SsoService"));
            
            // HealthChecks
            services.AddHealthChecks().AddDbContextCheck<DemoContext>();
        }

        private static void ConfigureDatabases(IServiceCollection services, IConfiguration configuration)
        {
            var databaseConnectionSettings = new DbConnectionModel();
            configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

            services.AddDbContextPool<DemoContext>(c =>
                c.UseSqlServer(CreateConnectionString(databaseConnectionSettings),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: new int[] { }
                    )).EnableDetailedErrors());
        }

        private static string CreateConnectionString(DbConnectionModel databaseConnectionModel)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = string.IsNullOrEmpty(databaseConnectionModel.Port)
                    ? databaseConnectionModel.Host
                    : databaseConnectionModel.Host + "," + databaseConnectionModel.Port,
                InitialCatalog = databaseConnectionModel.DatabaseName
            };

            if (databaseConnectionModel.IntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = databaseConnectionModel.UserName;
                builder.Password = databaseConnectionModel.Password;
            }

            builder.MultipleActiveResultSets = databaseConnectionModel.MultipleActiveResultSets;

            return builder.ConnectionString;
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
            //    Title = "Demo API"
            //});
        }

        public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheSettings = new RedisCacheModel();

            configuration.GetSection("RedisCacheSettings").Bind(redisCacheSettings);

            services.AddSingleton(redisCacheSettings);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisCacheSettings.ConnectionString;
                options.InstanceName = redisCacheSettings.InstanceName;
            });

            services.AddSingleton<Core.Services.IRedisCacheService, Infrastructure.Services.RedisCacheService>();
        }
    }
}