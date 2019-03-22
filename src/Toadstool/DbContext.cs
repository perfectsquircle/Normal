using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext
    {
        public DbContext()
        {
            DataReaderDeserializer = new DefaultDataReaderDeserializer();
        }

        public IDbTransaction Transaction { get; private set; }
        public IDataReaderDeserializer DataReaderDeserializer { get; private set; }
        private Func<IDbConnection> _dbConnectionCreator;

        public DbContext WithConnection(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
            return this;
        }

        public DbContext WithDataRowDeserializer(IDataReaderDeserializer dataReaderDeserializer)
        {
            DataReaderDeserializer = dataReaderDeserializer;
            return this;
        }

        public DbCommandBuilder Query(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbContext(this)
                .WithCommandText(commandText);
        }

        public async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }

            if (Transaction != null && Transaction.Connection != null)
            {
                return Transaction.Connection;
            }
            else
            {
                return await GetAnonymousConnectionAsync(cancellationToken);
            }
        }

        private async Task<IDbConnection> GetAnonymousConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = _dbConnectionCreator.Invoke();
            await OpenConnectionAsync(connection, cancellationToken);
            return connection;
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Transaction == null)
            {
                Transaction = (await GetAnonymousConnectionAsync(cancellationToken)).BeginTransaction();
            }
            return Transaction;
        }

        private static async Task OpenConnectionAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            if (!(connection is DbConnection))
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            var dbConnection = connection as DbConnection;
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }

        /*
        private static readonly int _connectionWaitRetries = 20;
        private static readonly int _connectionWaitPeriodMilliseconds = 100;
        private static async Task<IDbConnection> WaitForOpenConnection(IDbConnection connection, CancellationToken cancellationToken)
        {
            for (var i = 0; i < _connectionWaitRetries; i++)
            {
                var exponentialBackoffTimeout = _connectionWaitPeriodMilliseconds * Math.Pow(2, i);
                Debug.WriteLine($"Connection is busy, waiting {exponentialBackoffTimeout}ms");
                await Task.Delay((int)Math.Pow(i, _connectionWaitPeriodMilliseconds), cancellationToken);
                if (connection.State == ConnectionState.Open)
                {
                    return connection;
                }
            }
            throw new InvalidOperationException($"Timeout exceeded while waiting for open connection. Connection state: {Connection.State}");
        }
        */
    }
}