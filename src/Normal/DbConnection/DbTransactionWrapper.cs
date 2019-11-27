using System.Data;
using System.Data.Common;

namespace Normal
{
    internal class DbTransactionWrapper : IDbTransaction
    {
        private DbConnectionWrapper _connection;
        private event OnDispose _onDispose;
        public IDbConnection Connection => _connection?.DbConnection;
        private IDbTransaction DbTransaction => _connection?.DbTransaction;
        public IsolationLevel IsolationLevel { get; private set; }
        internal bool Enlisted => _connection != null;
        internal IDbConnectionWrapper ConnectionWrapper => _connection;


        public void Commit() => DbTransaction?.Commit();

        public void Rollback() => DbTransaction?.Rollback();

        public void Dispose()
        {
            // This is called by the end user when their transaction is complete.
            _onDispose();
            _connection?.Dispose(true);
        }

        internal DbTransactionWrapper WithIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }

        internal DbTransactionWrapper OnDispose(OnDispose onDispose)
        {
            _onDispose += onDispose;
            return this;
        }

        internal void Enlist(DbConnection dbConnection)
        {
            _connection = new DbConnectionWrapper(dbConnection, dbConnection.BeginTransaction(IsolationLevel));
        }
    }

    public delegate void OnDispose();
}