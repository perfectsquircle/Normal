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
        }

        public DbContext(Func<IDbConnection> dbConnectionCreator) : this()
        {
            _dbConnectionCreator = dbConnectionCreator;
        }

        internal IDbConnectionWrapper _activeDbConnectionWrapper;
        private Func<IDbConnection> _dbConnectionCreator;

        public DbContext WithConnection(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
            return this;
        }

        public IDbCommandBuilder Command(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbContext(this)
                .WithCommandText(commandText);
        }

        public async Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_activeDbConnectionWrapper != null)
            {
                CleanupActiveConnection();
            }
            var dbConnection = await GetAnonymousConnectionAsync(cancellationToken);
            var transaction = dbConnection.BeginTransaction();
            var transactionWrapper = new DbTransactionWrapper(transaction, CleanupActiveConnection);
            _activeDbConnectionWrapper = new DbConnectionWrapper(dbConnection, transactionWrapper);
            return transactionWrapper;
        }

        public void Dispose()
        {
            CleanupActiveConnection();
        }

        internal virtual async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }

            if (_activeDbConnectionWrapper != null)
            {
                return _activeDbConnectionWrapper;
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

        private void CleanupActiveConnection()
        {
            _activeDbConnectionWrapper?.Dispose();
            _activeDbConnectionWrapper = null;
        }
    }
}