using System;
using System.Data;

namespace Toadstool
{
    internal class DbTransactionWrapper : DbConnectionWrapper
    {
        public DbTransactionWrapper(IDbConnection dbConnection, IDbTransaction dbTransaction) : base(dbConnection)
        {
            DbTransaction = dbTransaction;
        }

        public IDbTransaction DbTransaction { get; private set; }

        public override IDbCommand CreateCommand()
        {
            var command = DbConnection.CreateCommand();
            command.Connection = DbConnection;
            command.Transaction = DbTransaction;
            return command;
        }

        public override void Dispose()
        {
            // This is called by DbCommandBuilder. We want the connection to
            // remain open, so do nothing.
            Dispose(false);

        }

        public void Dispose(bool force)
        {
            if (force)
            {
                DbTransaction?.Dispose();
                DbConnection?.Dispose();
            }
        }
    }

    internal class PublicDbTransactionWrapper : IDbTransaction
    {
        private OnDispose _onDispose;
        private DbTransactionWrapper _wrapper;
        private IDbTransaction DbTransaction => _wrapper?.DbTransaction;
        public IDbConnection Connection => _wrapper?.DbConnection;
        public IsolationLevel IsolationLevel { get; private set; }

        public void Commit() => DbTransaction?.Commit();

        public void Rollback() => DbTransaction?.Rollback();

        public void Dispose()
        {
            // This is called by the end user when their transaction is complete.
            _onDispose.Invoke();
            _wrapper?.Dispose(true);
        }

        internal bool Enlisted => _wrapper != null;
        internal IDbConnectionWrapper Wrapper => _wrapper;

        internal PublicDbTransactionWrapper WithIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }

        internal PublicDbTransactionWrapper WithOnDispose(OnDispose onDispose)
        {
            _onDispose = onDispose;
            return this;
        }

        internal void Enlist(IDbConnection dbConnection)
        {
            _wrapper = new DbTransactionWrapper(dbConnection, dbConnection.BeginTransaction(IsolationLevel));
        }
    }

    public delegate void OnDispose();
}