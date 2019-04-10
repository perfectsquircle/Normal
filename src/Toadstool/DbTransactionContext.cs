using System.Data;

namespace Toadstool
{
    public class DbTransactionContext : IDbTransactionContext
    {
        internal IDbTransaction DbTransaction { get; }

        internal DbTransactionContext(IDbTransaction dbTransaction)
        {
            DbTransaction = dbTransaction;
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
        }
    }
}