using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext, IDbConnectionProvider
    {
        private CreateConnection _createConnection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly AsyncLocal<DbTransactionWrapper> _currentTransaction = new AsyncLocal<DbTransactionWrapper>();
        internal DbTransactionWrapper CurrentTransaction { get { return _currentTransaction.Value; } set { _currentTransaction.Value = value; } }

        public DbContext()
        {
        }

        public DbContext(CreateConnection createConnection)
            : this()
        {
            _createConnection = createConnection;
        }

        public DbContext WithCreateConnection(CreateConnection createConnection)
        {
            _createConnection = createConnection;
            return this;
        }

        public IDbCommandBuilder CreateCommand(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbConnectionProvider(this)
                .WithCommandText(commandText);
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                _semaphore.Wait();
                if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException("Transaction already in progress!");
                }
                CurrentTransaction = new DbTransactionWrapper()
                    .WithIsolationLevel(isolationLevel)
                    .WithOnDispose(() => CurrentTransaction = null);
                return CurrentTransaction;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync();
                var transaction = CurrentTransaction;
                if (transaction != null)
                {
                    if (!transaction.Enlisted)
                    {
                        var dbConnection = await CreateOpenConnectionAsync(cancellationToken);
                        transaction.Enlist(dbConnection);
                    }
                    return transaction.ConnectionWrapper;
                }
                else
                {
                    return new DbConnectionWrapper(await CreateOpenConnectionAsync(cancellationToken));
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_createConnection == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }
            var connection = _createConnection.Invoke();
            if (connection == null)
            {
                throw new InvalidOperationException("Connection is null");
            }
            if (!(connection is DbConnection))
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            await (connection as DbConnection).OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            CurrentTransaction = null;
            _semaphore?.Dispose();
        }
    }

    public delegate IDbConnection CreateConnection();
}