using System;

namespace Toadstool
{
    public interface IDbTransactionContext : IDisposable
    {
        void Commit();
        void Rollback();
        bool IsComplete { get; }
    }
}