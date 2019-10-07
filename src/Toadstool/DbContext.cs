using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext, IDbConnectionProvider
    {
        private CreateConnection _dbConnectionCreator;
        private readonly AsyncLocal<PublicDbTransactionWrapper> _currentTransaction = new AsyncLocal<PublicDbTransactionWrapper>();
        internal PublicDbTransactionWrapper CurrentTransaction { get { return _currentTransaction.Value; } set { _currentTransaction.Value = value; } }

        public DbContext()
        {
        }

        public DbContext(CreateConnection dbConnectionCreator)
            : this()
        {
            _dbConnectionCreator = dbConnectionCreator;
        }

        public DbContext WithConnection(CreateConnection dbConnectionCreator)
        {
            _dbConnectionCreator = dbConnectionCreator;
            return this;
        }

        public IDbCommandBuilder CreateCommand(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbConnectionProvider(this)
                .WithCommandText(commandText);
        }

        public IDbTransaction BeginTransaction()
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException("Transaction already in progress!");
            }
            var publicDbTransactionWrapper = new PublicDbTransactionWrapper(() => CurrentTransaction = null);
            CurrentTransaction = publicDbTransactionWrapper;
            return publicDbTransactionWrapper;
        }

        internal async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var transaction = CurrentTransaction;
            if (transaction != null)
            {
                if (transaction.Wrapper == null)
                {
                    var dbConnection = await CreateOpenConnectionAsync(cancellationToken);
                    transaction.Wrapper = new DbTransactionWrapper(dbConnection, dbConnection.BeginTransaction());
                }
                return transaction.Wrapper;
            }
            else
            {
                return new DbConnectionWrapper(await CreateOpenConnectionAsync(cancellationToken));
            }
        }

        private async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnectionCreator == null)
            {
                throw new InvalidOperationException("No DB Connection Creator");
            }
            var connection = _dbConnectionCreator.Invoke();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            CurrentTransaction = null;
        }
    }

    public delegate DbConnection CreateConnection();
}