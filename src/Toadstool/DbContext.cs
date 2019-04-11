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
            DataRecordDeserializer = new DefaultDataRecordDeserializer();
        }

        internal IDataRecordDeserializer DataRecordDeserializer { get; private set; }
        internal IDbConnectionWrapper _activeDbConnectionContext;
        private Func<IDbConnection> _dbConnectionCreator;

        public DbContext WithConnection(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
            return this;
        }

        public DbContext WithDataRecordDeserializer(IDataRecordDeserializer dataRecordDeserializer)
        {
            DataRecordDeserializer = dataRecordDeserializer;
            return this;
        }

        public IDbCommandBuilder Query(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbContext(this)
                .WithCommandText(commandText);
        }

        public async Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_activeDbConnectionContext != null)
            {
                CleanupActiveContext();
            }
            var dbConnection = await GetAnonymousConnectionAsync(cancellationToken);
            var transaction = dbConnection.BeginTransaction();
            var transactionContext = new DbTransactionWrapper(transaction, CleanupActiveContext);
            _activeDbConnectionContext = new DbConnectionWrapper(dbConnection, transactionContext);
            return transactionContext;
        }

        public void Dispose()
        {
            CleanupActiveContext();
        }

        internal virtual async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }

            if (_activeDbConnectionContext != null)
            {
                return _activeDbConnectionContext;
            }
            else
            {
                return new DbConnectionWrapper(await GetAnonymousConnectionAsync(cancellationToken));
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

        private void CleanupActiveContext()
        {
            _activeDbConnectionContext?.Dispose();
            _activeDbConnectionContext = null;
        }
    }
}