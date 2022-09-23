using System.Reflection;
using DbUp;
using Demo.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Configurations
{
    public class DatabaseScriptInit : IStartupFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public DatabaseScriptInit(IConfiguration configuration, ILogger<DatabaseScriptInit> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            var databaseConnectionSettings = new DbConnectionModel();
            _configuration.GetSection("DbConnectionSettings").Bind(databaseConnectionSettings);

            var dbUpgradeEngine = DeployChanges.To
                .SqlDatabase(databaseConnectionSettings.CreateConnectionString(databaseConnectionSettings.Write))
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .JournalToSqlTable("dbo", "SchemaVersions")
                .LogToConsole()
                .Build();

            if (!dbUpgradeEngine.IsUpgradeRequired()) return next;
            _logger.LogInformation("Database migration have been detected. Upgrading database now...");
            var operation = dbUpgradeEngine.PerformUpgrade();
            _logger.LogInformation(operation.Successful
                ? "Database migration completed successfully"
                : "Error happened in the database migration. Please check the logs");
            return next;
        }
    }
}
