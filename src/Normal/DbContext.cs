using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public partial class DbContext : IDbContext
    {
        private CreateConnection _createConnection;
        private IHandler _handler;
        private readonly AsyncLocal<DbTransactionWrapper> _currentTransaction;
        private readonly SemaphoreSlim _semaphore;
        internal DbTransactionWrapper CurrentTransaction
        {
            get { return _currentTransaction.Value; }
            private set { _currentTransaction.Value = value; }
        }

        public DbContext(CreateConnection createConnection) : this()
        {
            _createConnection = createConnection;
        }

        public DbContext(Action<IDbContextBuilder> configure) : this()
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var dbContextBuilder = new DbContextBuilder();
            configure(dbContextBuilder);
            _createConnection = dbContextBuilder.CreateConnection;
            _handler = dbContextBuilder.BuildHandler(this);
        }

        public DbContext(Type connectionType, params object[] arguments) : this()
        {
            var constructor = ReflectionHelper.GetConstructor(connectionType, arguments);
            _createConnection = () => (IDbConnection)constructor.Invoke(arguments);
        }

        private DbContext()
        {
            _handler = new BaseHandler(this);
            _currentTransaction = new AsyncLocal<DbTransactionWrapper>();
            _semaphore = new SemaphoreSlim(1, 1);
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
                    .OnDispose(() => CurrentTransaction = null);
                return CurrentTransaction;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        internal async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
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

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            CurrentTransaction = null;
            _semaphore?.Dispose();
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
    }
}