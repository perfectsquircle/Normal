using System;

namespace Normal
{
    public interface IDbTransactionWrapper : IDisposable
    {
        void Commit();
        void Rollback();
    }
}