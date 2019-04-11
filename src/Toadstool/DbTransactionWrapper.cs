using System;
using System.Data;

namespace Toadstool
{
    public class DbTransactionWrapper : IDbTransactionWrapper
    {
        internal IDbTransaction DbTransaction { get; }
        private Action _onDisposed;

        internal DbTransactionWrapper(IDbTransaction dbTransaction, Action onDisposed)
        {
            DbTransaction = dbTransaction;
            _onDisposed = onDisposed;
        }

        public bool IsComplete { get; private set; }

        public void Commit()
        {
            IsComplete = true;
            DbTransaction?.Commit();
        }

        public void Rollback()
        {
            IsComplete = true;
            DbTransaction?.Rollback();
        }

        public void Dispose()
        {
            IsComplete = true;
            DbTransaction?.Dispose();
            _onDisposed?.Invoke();
        }
    }
}