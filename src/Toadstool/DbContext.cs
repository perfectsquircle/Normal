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

        internal IDataReaderDeserializer DataReaderDeserializer { get; private set; }
        private Func<IDbConnection> _dbConnectionCreator;
        private IDbConnectionContext _activeDbConnectionContext;

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

        public async Task<IDbTransactionContext> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_activeDbConnectionContext != null)
            {
                CleanupActiveContext();
            }
            var dbConnection = await GetAnonymousConnectionAsync(cancellationToken);
            var transaction = dbConnection.BeginTransaction();
            var transactionContext = new DbTransactionContext(transaction);
            _activeDbConnectionContext = new DbConnectionContext(dbConnection, transactionContext);
            return transactionContext;
        }

        public void Dispose()
        {
            CleanupActiveContext();
        }

        internal virtual async Task<IDbConnectionContext> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }

            if (_activeDbConnectionContext != null && _activeDbConnectionContext.IsComplete)
            {
                CleanupActiveContext();
            }

            if (_activeDbConnectionContext != null)
            {
                return _activeDbConnectionContext;
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

        private void CleanupActiveContext()
        {
            _activeDbConnectionContext?.Dispose();
            _activeDbConnectionContext = null;
        }
    }
}