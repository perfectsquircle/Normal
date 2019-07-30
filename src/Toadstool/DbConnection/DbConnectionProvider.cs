using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal class DbConnectionProvider : IDbConnectionProvider
    {
        private Func<IDbConnection> _dbConnectionCreator;
        private DbConnectionWrapper _activeDbConnectionWrapper;

        public DbConnectionProvider(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
        }

        public async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_activeDbConnectionWrapper != null)
            {
                return _activeDbConnectionWrapper;
            }
            else
            {
                return new DbConnectionWrapper(await GetAnonymousConnectionAsync(cancellationToken));
            }
        }

        public async Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_activeDbConnectionWrapper != null)
            {
                throw new InvalidOperationException("Transaction already in progress");
            }
            var dbConnection = await GetAnonymousConnectionAsync(cancellationToken);
            var transaction = dbConnection.BeginTransaction();
            _activeDbConnectionWrapper = new DbConnectionWrapper(dbConnection, transaction);
            return new DbTransactionWrapper(transaction, CleanupActiveConnection);
        }

        public void Dispose()
        {
            CleanupActiveConnection();
        }

        private async Task<IDbConnection> GetAnonymousConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }
            var connection = _dbConnectionCreator.Invoke();
            if (!(connection is DbConnection))
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            var dbConnection = connection as DbConnection;
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        private void CleanupActiveConnection()
        {
            try
            {
                _activeDbConnectionWrapper?.Dispose(true);
            }
            finally
            {
                _activeDbConnectionWrapper = null;
            }
        }

        private class DbTransactionWrapper : IDbTransactionWrapper
        {
            private readonly IDbTransaction _transaction;
            private Action _onDisposed;

            public DbTransactionWrapper(IDbTransaction connection, Action onDisposed)
            {
                _transaction = connection;
                _onDisposed = onDisposed;
            }

            public void Commit()
            {
                _transaction.Commit();
            }

            public void Rollback()
            {
                _transaction.Rollback();
            }

            public void Dispose()
            {
                _onDisposed.Invoke();
            }
        }
    }
}