using System.Data;
using System.Data.Common;

namespace Normal
{
    internal class DbTransactionWrapper : IDbTransaction
    {
        private DbConnectionWrapper _connection;
        private IDbTransaction DbTransaction => _connection?.DbTransaction;
        public IDbConnection Connection => _connection?.DbConnection;
        public IsolationLevel IsolationLevel { get; private set; }
        public event OnDispose OnDispose;

        public void Commit() => DbTransaction?.Commit();

        public void Rollback() => DbTransaction?.Rollback();

        public void Dispose()
        {
            // This is called by the end user when their transaction is complete.
            OnDispose();
            _connection?.Dispose(true);
        }

        internal bool Enlisted => _connection != null;
        internal IDbConnectionWrapper ConnectionWrapper => _connection;

        internal DbTransactionWrapper WithIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }

        internal DbTransactionWrapper WithOnDispose(OnDispose onDispose)
        {
            OnDispose += onDispose;
            return this;
        }

        internal void Enlist(DbConnection dbConnection)
        {
            _connection = new DbConnectionWrapper(dbConnection, dbConnection.BeginTransaction(IsolationLevel));
        }
    }

    public delegate void OnDispose();
}