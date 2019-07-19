using System;

namespace Toadstool
{
    public interface IDbTransactionWrapper : IDisposable
    {
        void Commit();
        void Rollback();
    }
}