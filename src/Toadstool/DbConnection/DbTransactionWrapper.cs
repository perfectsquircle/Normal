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
        private OnDisposed _onDisposed;
        private IDbTransaction DbTransaction => Wrapper?.DbTransaction;

        public PublicDbTransactionWrapper(OnDisposed onDisposed)
        {
            _onDisposed = onDisposed;
        }

        internal DbTransactionWrapper Wrapper { get; set; }
        public IDbConnection Connection => Wrapper?.DbConnection;
        public IsolationLevel IsolationLevel => DbTransaction.IsolationLevel;

        public void Commit() => DbTransaction?.Commit();

        public void Rollback() => DbTransaction?.Rollback();


        public void Dispose()
        {
            // This is called by the end user when their transaction is complete.
            _onDisposed.Invoke();
            Wrapper?.Dispose(true);
        }
    }

    public delegate void OnDisposed();
}