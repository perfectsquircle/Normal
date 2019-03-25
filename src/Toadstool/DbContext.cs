using System;
using System.Data;
using System.Data.Common;
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
                Transaction?.Connection?.Dispose();
                Transaction?.Dispose();
                Transaction = null;
            }

        }

        internal virtual async Task<IDbConnectionContext> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }

            if (Transaction != null && Transaction.Connection != null)
            {
                return new DbConnectionContext(Transaction.Connection, Transaction);
            }
            else
            {
                return new DbConnectionContext(await GetAnonymousConnectionAsync(cancellationToken));
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Transaction?.Connection?.Dispose();
                    Transaction?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}