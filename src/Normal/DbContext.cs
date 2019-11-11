using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public class DbContext : IDbContext
    {
        internal CreateConnection CreateConnection { get; set; }
        internal IHandler Handler { get; set; }
        internal DataRecordMapperFactory DataRecordMapperFactory { get; }
        internal DbTransactionWrapper CurrentTransaction
        {
            get { return _currentTransaction.Value; }
            private set { _currentTransaction.Value = value; }
        }
        private readonly AsyncLocal<DbTransactionWrapper> _currentTransaction;
        private readonly SemaphoreSlim _semaphore;

        public DbContext(CreateConnection createConnection)
            : this()
        {
            CreateConnection = createConnection;
        }

        internal DbContext()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _currentTransaction = new AsyncLocal<DbTransactionWrapper>();
            Handler = new BaseHandler(this);
            DataRecordMapperFactory = new DataRecordMapperFactory();
        }

        public static DbContext Build(Action<IDbContextBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var dbContextBuilder = new DbContextBuilder();
            configure(dbContextBuilder);
            return dbContextBuilder.Build();
        }

        public IDbCommandBuilder CreateCommand(string commandText)
        {
            return new DbCommandBuilder()
                .WithHandler(Handler)
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

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            CurrentTransaction = null;
            _semaphore?.Dispose();
        }

        internal DbContext UseConnection(CreateConnection createConnection)
        {
            CreateConnection = createConnection;
            return this;
        }

        private async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (CreateConnection == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }
            var connection = CreateConnection.Invoke();
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