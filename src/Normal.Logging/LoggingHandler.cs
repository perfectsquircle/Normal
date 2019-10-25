using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Normal
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public LoggingHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override async Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken)
        {
            var rowsAffected = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                rowsAffected = await InnerHandler.ExecuteNonQueryAsync(command, cancellationToken);
                return rowsAffected;
            }
            finally
            {
                var parameters = command.Parameters.Cast<DbParameter>().ToDictionary(p => p.ParameterName, p => p.Value);
                _logger.LogInformation("non-query: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms\n\trows affected: {rows affected}",
                    command.CommandText, parameters, stopwatch.ElapsedMilliseconds, rowsAffected);
            }
        }

        public override async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var reader = await InnerHandler.ExecuteReaderAsync(command, cancellationToken);
                return reader;
            }
            finally
            {
                var parameters = command.Parameters.Cast<DbParameter>().ToDictionary(p => p.ParameterName, p => p.Value);
                _logger.LogInformation("query: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms",
                    command.CommandText, parameters, stopwatch.ElapsedMilliseconds);
            }
        }

        public override async Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken)
        {
            object result = default(object);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                result = await InnerHandler.ExecuteScalarAsync(command, cancellationToken);
                return result;
            }
            finally
            {
                var parameters = command.Parameters.Cast<DbParameter>().ToDictionary(p => p.ParameterName, p => p.Value);
                _logger.LogInformation("scalar: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms\n\rresult: {result}",
                    command.CommandText, parameters, stopwatch.ElapsedMilliseconds, result);
            }
        }
    }
}