using System.Data;
using System.Data.Common;

namespace Demo.Util.FIQL
{
    public class CountingDbCommands : DbCommand
    {
        private readonly DbCommand _command;
        private readonly Action _onExecute;

        public CountingDbCommands(DbCommand command, Action onExecute)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _onExecute = onExecute;
        }

        // Override async methods for async Dapper operations
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            _onExecute?.Invoke();
            return _command.ExecuteNonQueryAsync(cancellationToken);
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            _onExecute?.Invoke();
            return _command.ExecuteScalarAsync(cancellationToken);
        }

        // Synchronous methods also need to increment the query count
        public override int ExecuteNonQuery()
        {
            _onExecute?.Invoke();
            return _command.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            _onExecute?.Invoke();
            return _command.ExecuteScalar();
        }

        // Other necessary members of DbCommand
        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }

        protected override DbParameter CreateDbParameter() => _command.CreateParameter();

        public override void Cancel() => _command.Cancel();
        public override void Prepare() => _command.Prepare();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _onExecute?.Invoke();
            return _command.ExecuteReader(behavior);
        }
    }

}