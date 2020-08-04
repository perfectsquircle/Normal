using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public partial class Database : IDatabase, IDbConnectionProvider
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

        public Variant Variant { get; set; }

        internal Database(CreateConnection createConnection) : this()
        {
            _createConnection = createConnection;
        }

        public Database(Action<IDatabaseBuilder> configure) : this()
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var databaseBuilder = new DatabaseBuilder();
            configure(databaseBuilder);
            _createConnection = databaseBuilder.CreateConnection;
            _handler = databaseBuilder.BuildHandler(this);
            Variant = databaseBuilder.Variant;
        }

        public static Database WithConnection<TConnection>(params object[] arguments)
        {
            return new Database(typeof(TConnection), arguments);
        }

        private Database(Type connectionType, params object[] arguments) : this()
        {
            var constructor = ReflectionHelper.GetConstructor(connectionType, arguments);
            _createConnection = () => (IDbConnection)constructor.Invoke(arguments);
            Variant = DetermineVariant(connectionType);
        }


        private Database()
        {
            _handler = new BaseHandler(this);
            _currentTransaction = new AsyncLocal<DbTransactionWrapper>();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        internal static Variant DetermineVariant(Type connectionType)
        {
            var typeAsString = connectionType?.ToString() ?? "";
            if (typeAsString.EndsWith("NpgsqlConnection")) return Variant.PostgreSQL;
            if (typeAsString.EndsWith("SqlConnection")) return Variant.SQLServer;
            if (typeAsString.EndsWith("MySqlConnection")) return Variant.MySQL;
            if (typeAsString.EndsWith("OracleClient")) return Variant.Oracle;
            return Variant.Unknown;
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