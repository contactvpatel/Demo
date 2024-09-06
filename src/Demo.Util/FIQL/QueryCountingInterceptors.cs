using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Util.FIQL
{
    public class DbConnectionInterceptors : DbConnection
    {
        private readonly DbConnection _connection;
        private int _queryCount;

        public DbConnectionInterceptors(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public int GetQueryCount()
        {
            return _queryCount;
        }

        private void IncrementQueryCount()
        {
            _queryCount++;
        }

        protected override DbCommand CreateDbCommand()
        {
            // Ensure that CreateDbCommand returns a DbCommand (not IDbCommand)
            var command = _connection.CreateCommand();
            return new CountingDbCommands(command, IncrementQueryCount);
        }

        // Implement the required BeginDbTransaction method
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            // Delegate the transaction creation to the underlying connection
            return _connection.BeginTransaction(isolationLevel);
        }

        // Delegate all other DbConnection methods to the wrapped connection
        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public override string Database => _connection.Database;
        public override ConnectionState State => _connection.State;
        public override string DataSource => _connection.DataSource;
        public override string ServerVersion => _connection.ServerVersion;

        public override void Open() => _connection.Open();
        public override void Close() => _connection.Close();
        public override Task OpenAsync(CancellationToken cancellationToken) => _connection.OpenAsync(cancellationToken);
        public override void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);
    }


}