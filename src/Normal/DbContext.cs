using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public class DbContext : IDbContext, IDbConnectionProvider
    {
        private CreateConnection _createConnection;
        private readonly SemaphoreSlim _semaphore;
        private readonly AsyncLocal<DbTransactionWrapper> _currentTransaction;
        private IHandler _handler;

        internal DbTransactionWrapper CurrentTransaction { get { return _currentTransaction.Value; } set { _currentTransaction.Value = value; } }

        public DbContext()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _currentTransaction = new AsyncLocal<DbTransactionWrapper>();
            _handler = new BaseHandler(this);
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
                .WithHandler(_handler)
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

        private async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
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
            var dbConnection = connection as DbConnection;
            if (dbConnection == null)
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return dbConnection;
        }

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            CurrentTransaction = null;
            _semaphore?.Dispose();
        }

        internal DbContext WithHandler(IHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            _handler = handler;
            return this;
        }
    }
}