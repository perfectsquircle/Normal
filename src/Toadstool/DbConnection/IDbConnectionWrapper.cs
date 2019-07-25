using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDbTransactionWrapper, IDisposable
    {
        IDbCommand CreateCommand();
        CommandBehavior CommandBehavior { get; }
    }
}