using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Demo.Util.FIQL
{
    public class QueryCountInterceptor : DbCommandInterceptor
    {
        private readonly QueryTrackerService _queryTracker;

        public QueryCountInterceptor(QueryTrackerService queryTracker)
        {
            _queryTracker = queryTracker;
        }

        private void IncrementQueryCount()
        {
            _queryTracker.Increment();
        }

        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            IncrementQueryCount();
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            IncrementQueryCount();
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            IncrementQueryCount();
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            IncrementQueryCount();
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            IncrementQueryCount();
            return base.ScalarExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            IncrementQueryCount();
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}