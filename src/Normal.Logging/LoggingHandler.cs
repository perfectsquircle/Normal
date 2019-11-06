using System.Collections.Generic;
using System.Diagnostics;
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

        public override async Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            var rowsAffected = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                rowsAffected = await InnerHandler.ExecuteNonQueryAsync(commandBuilder, cancellationToken);
                return rowsAffected;
            }
            finally
            {
                var parameters = commandBuilder.Parameters;
                _logger.LogInformation("non-query: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms\n\trows affected: {rows affected}",
                    commandBuilder.CommandText, parameters, stopwatch.ElapsedMilliseconds, rowsAffected);
            }
        }

        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var results = await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
                return results;
            }
            finally
            {
                var parameters = commandBuilder.Parameters;
                _logger.LogInformation("query: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms",
                    commandBuilder.CommandText, parameters, stopwatch.ElapsedMilliseconds);
            }
        }

        public override async Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            T result = default(T);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                result = await InnerHandler.ExecuteScalarAsync<T>(commandBuilder, cancellationToken);
                return result;
            }
            finally
            {
                var parameters = commandBuilder.Parameters;
                _logger.LogInformation("scalar: {commandText}\n\tparameters: {parameters}\n\telapsed: {ElapsedMilliseconds}ms\n\rresult: {result}",
                    commandBuilder.CommandText, parameters, stopwatch.ElapsedMilliseconds, result);
            }
        }
    }
}