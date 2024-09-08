using Asp.Versioning;
using Demo.Business.Interfaces;
using Demo.Business.Services;
using Demo.Core.Models;
using Demo.Core.Repositories;
using Demo.Core.Repositories.Base;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repositories;
using Demo.Infrastructure.Repositories.Base;
using Demo.Util.FIQL;
using Demo.Util.Logging;
using Demo.Util.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestSharp;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Data.SqlClient;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Demo.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDemoServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database
            ConfigureDatabases(services, configuration);

            // Add Infrastructure Layer
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ISalesOrderHeaderRepository, SalesOrderHeaderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<Core.Services.IAsmService, Infrastructure.Services.AsmService>();
            services.AddScoped<Core.Services.IMisService, Infrastructure.Services.MisService>();
            services.AddScoped<Core.Services.ISsoService, Infrastructure.Services.SsoService>();
            services.AddScoped<IResponseToDynamic, ResponseToDynamic>();

            // Add Business Layer
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ISalesOrderHeaderService, SalesOrderHeaderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAsmService, AsmService>();
            services.AddScoped<IMisService, MisService>();
            services.AddScoped<ISsoService, SsoService>();

            services.AddScoped<QueryTrackerService>();
            services.AddScoped<QueryCountInterceptor>();

            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // LoggingHelpers
            services.AddTransient<LoggingDelegatingHandler>();

            //External Service Dependency (Example: MisService, SsoService, AsmService)
            services.AddScoped<RestClient>();
            services.Configure<MisApiModel>(configuration.GetSection("MisService"));
            services.Configure<SsoApiModel>(configuration.GetSection("SsoService"));
            services.Configure<AsmApiModel>(configuration.GetSection("AsmService"));

            // Configure AppSettings Object
            services.Configure<AppSettingModel>(configuration.GetSection("AppSettings"));

            // HealthChecks
            services.AddHealthChecks().AddDbContextCheck<DemoReadContext>().AddDbContextCheck<DemoWriteContext>();
        }

        private static void ConfigureDatabases(IServiceCollection services, IConfiguration configuration)
        {
            var databaseConnectionSettings = new DbConnectionModel();
            configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

            services.AddDbContext<DemoReadContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Read),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: []
                    ));
                options.EnableDetailedErrors();
                options.AddInterceptors(serviceProvider.GetRequiredService<QueryCountInterceptor>());
            });

            services.AddDbContext<DemoWriteContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Write),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).EnableRetryOnFailure(
                        maxRetryCount: 4,
                        maxRetryDelay: TimeSpan.FromSeconds(1),
                        errorNumbersToAdd: []
                    ));
                options.EnableDetailedErrors();
                options.AddInterceptors(serviceProvider.GetRequiredService<QueryCountInterceptor>());
            });

            // Register IDbConnection with QueryCountingInterceptor wrapper
            services.AddScoped<IDbConnection>(serviceProvider =>
            {
                // Get the original connection (e.g., SqlConnection)
                var originalConnection = new SqlConnection(DbConnectionModel.CreateConnectionString(databaseConnectionSettings.Read));
                // Wrap it with the QueryCountingInterceptor
                return new DbConnectionInterceptors(originalConnection);
            });
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

                // Allow versioning via URL and headers
                options.ApiVersionReader = ApiVersionReader.Combine(
                    // URL versioning (e.g., api/v1/[controller])
                    new UrlSegmentApiVersionReader(),
                    // Header versioning (e.g., x-app-api-version: 1)
                    new HeaderApiVersionReader("x-app-api-version")
                );

            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(ConfigureSwaggerGen);
        }

        private static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            AddSwaggerDocs(options);

            // Add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        }

        private static void AddSwaggerDocs(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
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

            if (redisCacheSettings.Enabled)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = redisCacheSettings.InstanceName;
                    options.ConfigurationOptions = ConfigurationOptions.Parse(redisCacheSettings.ConnectionString);
                    if (options.ConfigurationOptions != null)
                        options.ConfigurationOptions.CertificateValidation += ValidateServerCertificate;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddSingleton<Core.Services.IRedisCacheService, Infrastructure.Services.RedisCacheService>();
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }
    }
}