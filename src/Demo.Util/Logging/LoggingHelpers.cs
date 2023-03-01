using System.Reflection;
using Demo.Util.Logging.Enrichers;
using Demo.Util.Logging.Formatters;
using Demo.Util.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;

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

            var appSettings = new AppSettingModel();
            config.GetSection("AppSettings").Bind(appSettings);

            loggerConfiguration
                .ReadFrom.Configuration(config) // minimum log levels defined per project in appsettings.json files 
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", appSettings.ApplicationName)
                .Enrich.WithProperty("ApplicationVersion", appSettings.ApplicationVersion)
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .Enrich.WithProperty("LoggerName", assembly.Name ?? string.Empty)
                .Enrich.WithHttpContextInfo(provider, (logEvent, propertyFactory, httpContext) =>
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod",
                        httpContext.Request.Method));
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Referer",
                        httpContext.Request.Headers["Referer"].ToString()));
                    if (httpContext.Response.HasStarted)
                    {
                        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ResponseStatus",
                            httpContext.Response.StatusCode));
                    }
                })
                .Enrich.WithCorrelationId()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName();


            #region Write Logs to Console

            var isConsoleLogEnabled = config.GetValue<bool>("Serilog:ConsoleLog:Enabled");

            if (isConsoleLogEnabled)
                loggerConfiguration.WriteTo.Console(
                    new CustomElasticSearchJsonFormatter(inlineFields: true, renderMessageTemplate: false,
                        formatStackTraceAsArray: true));

            #endregion

            #region Write Logs to File

            var isFileLogEnabled = config.GetValue<bool>("Serilog:FileLog:Enabled");

            if (isFileLogEnabled)
            {
                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Matching.WithProperty("elapsedMilliseconds"))
                    .WriteTo.File(new CustomLogEntryFormatter(),
                        $"logs/File/performance-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-.txt",
                        rollingInterval: RollingInterval.Day));

                loggerConfiguration.WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(Matching.WithProperty("elapsedMilliseconds"))
                    .WriteTo.File(new CustomLogEntryFormatter(),
                        $"logs/File/usage-{env.ApplicationName.Replace(".", "-").ToLower()}-{env.EnvironmentName?.ToLower()}-.txt",
                        rollingInterval: RollingInterval.Day)
                );
            }

            #endregion
        }
    }
}