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

        internal IDbTransaction Transaction { get; private set; }
        internal IDataReaderDeserializer DataReaderDeserializer { get; private set; }
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

        public async Task BeginTransactionAsync(Func<IDbTransaction, Task> transactionAction, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (Transaction == null)
                {
                    Transaction = (await GetAnonymousConnectionAsync(cancellationToken)).BeginTransaction();
                }
                await transactionAction.Invoke(Transaction);
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
            }

        }

        internal virtual async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
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

        private static async Task OpenConnectionAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            if (!(connection is DbConnection))
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            var dbConnection = connection as DbConnection;
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}