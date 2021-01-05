using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;

namespace Demo.Util.Logging
{
    public static class LoggingHelpers
    {
        /// <summary>
        /// Provides standardized, centralized Serilog wire-up for a suite of applications.
        /// </summary>
        /// <param name="loggerConfiguration">Provide this value from the UseSerilog method param</param>
        /// <param name="hostingContext">Hosting Context</param>
        /// <param name="provider">Provider</param>
        /// <param name="config">IConfiguration settings -- generally read this from appsettings.json</param>
        public static void LogConfiguration(this LoggerConfiguration loggerConfiguration,
            HostBuilderContext hostingContext, IServiceProvider provider, IConfiguration config)
        {
            var env = hostingContext.HostingEnvironment;
            var assembly = Assembly.GetExecutingAssembly().GetName();

            loggerConfiguration
                .ReadFrom.Configuration(config) // minimum log levels defined per project in appsettings.json files 
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithProperty("Assembly", $"{assembly.Name}")
                .Enrich.WithProperty("Version", $"{assembly.Version}")
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithExceptionDetails();

            #region Write Logs to Console
            
            loggerConfiguration.WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Properties} {SourceContext} [{Level}] {Message}{NewLine}{Exception}");

            #endregion

            #region Write Logs to File
            
            loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.WithProperty("ElapsedMilliseconds"))
                .WriteTo.File(new JsonFormatter(),
                    $"logs/File/performance-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-.txt",
                    rollingInterval: RollingInterval.Day, buffered: true)
            );

            loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByExcluding(Matching.WithProperty("ElapsedMilliseconds"))
                .WriteTo.File(new JsonFormatter(),
                    $"logs/File/usage-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-.txt",
                    rollingInterval: RollingInterval.Day, buffered: true)
            );

            #endregion

            # region Write Logs to Elastic Search - https://github.com/serilog/serilog-sinks-elasticsearch
            
            var elasticUrl = config.GetValue<string>("Serilog:ElasticUrl");

            if (!string.IsNullOrEmpty(elasticUrl))
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(Matching.WithProperty("ElapsedMilliseconds"))
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl))
                            {
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                IndexFormat =
                                    $"performance-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-{{0:yyyy.MM.dd}}",
                                InlineFields = true,
                                CustomFormatter = new ElasticsearchJsonFormatter(inlineFields: true,
                                    renderMessageTemplate: false),
                                FailureCallback = e =>
                                    Console.WriteLine("Unable to submit log event to ElasticSearch. Log Level - " +
                                                      e.Level),
                                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                                   EmitEventFailureHandling.WriteToFailureSink |
                                                   EmitEventFailureHandling.RaiseCallback,
                                FailureSink =
                                    new FileSink(
                                        $"logs/FailureCallback/performance-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}/failures.txt",
                                        new JsonFormatter(), null)
                            }
                        ))
                    .WriteTo.Logger(lc => lc
                        .Filter.ByExcluding(Matching.WithProperty("ElapsedMilliseconds"))
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl))
                            {
                                AutoRegisterTemplate = true,
                                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                IndexFormat =
                                    $"usage-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-{{0:yyyy.MM.dd}}",
                                InlineFields = true,
                                CustomFormatter = new ElasticsearchJsonFormatter(inlineFields: true,
                                    renderMessageTemplate: false),
                                FailureCallback = e =>
                                    Console.WriteLine("Unable to submit log event to ElasticSearch. Log Level - " +
                                                      e.Level),
                                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                                   EmitEventFailureHandling.WriteToFailureSink |
                                                   EmitEventFailureHandling.RaiseCallback,
                                FailureSink =
                                    new FileSink(
                                        $"logs/FailureCallback/usage-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}/failures.txt",
                                        new JsonFormatter(), null)
                            }
                        ));
            }

            #endregion
        }

        // Future Use for Context Details
        private static UserInfo AddCustomContextDetails(IHttpContextAccessor ctx)
        {
            var excluded = new List<string> {"nbf", "exp", "auth_time", "amr", "sub"};
            const string userIdClaimType = "sub";

            var context = ctx.HttpContext;
            var user = context?.User.Identity;
            if (user == null || !user.IsAuthenticated) return null;

            var userId = context.User.Claims.FirstOrDefault(a => a.Type == userIdClaimType)?.Value;
            var userInfo = new UserInfo
            {
                UserName = user.Name,
                UserId = userId,
                UserClaims = new Dictionary<string, List<string>>()
            };
            foreach (var distinctClaimType in context.User.Claims
                .Where(a => excluded.All(ex => ex != a.Type))
                .Select(a => a.Type)
                .Distinct())
            {
                userInfo.UserClaims[distinctClaimType] = context.User.Claims
                    .Where(a => a.Type == distinctClaimType)
                    .Select(c => c.Value)
                    .ToList();
            }

            return userInfo;
        }
    }
}