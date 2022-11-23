using System.Data.SqlClient;

namespace Demo.Core.Models
{
    public class DbConnectionModel
    {
        public ConnectionModel Read { get; set; }
        public ConnectionModel Write { get; set; }

        public static string CreateConnectionString(ConnectionModel databaseConnectionModel)
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
            builder.TrustServerCertificate = true;

            return builder.ConnectionString;
        }
    }

    public class ConnectionModel
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string DatabaseName { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool MultipleActiveResultSets { get; set; }
    }
}