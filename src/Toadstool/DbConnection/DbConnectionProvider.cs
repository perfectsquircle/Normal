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
#pragma warning disable CC0033
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // TODO: dispose me
#pragma warning restore CC0033

        public DbConnectionProvider(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
        }

        public async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (_activeDbConnectionWrapper != null)
                {
                    return _activeDbConnectionWrapper;
                }
                else
                {
                    return new DbConnectionWrapper(await GetAnonymousConnectionAsync(cancellationToken));
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (_activeDbConnectionWrapper != null)
                {
                    CleanupActiveConnection();
                }
                var dbConnection = await GetAnonymousConnectionAsync(cancellationToken);
                var transaction = dbConnection.BeginTransaction();
                _activeDbConnectionWrapper = new DbConnectionWrapper(dbConnection, transaction);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Commit()
        {
            try
            {
                _semaphore.Wait();
                _activeDbConnectionWrapper?.Commit();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Rollback()
        {
            try
            {
                _semaphore.Wait();
                _activeDbConnectionWrapper?.Rollback();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            try
            {
                _semaphore.Wait();
                CleanupActiveConnection();
            }
            finally
            {
                _semaphore.Release();
            }
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

        private async Task<IDbConnection> GetAnonymousConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }
            var connection = _dbConnectionCreator.Invoke();
            await OpenConnectionAsync(connection, cancellationToken);
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
    }
}