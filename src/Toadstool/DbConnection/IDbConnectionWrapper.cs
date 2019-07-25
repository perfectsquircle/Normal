using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDbTransactionWrapper, IDisposable
    {
        CommandBehavior CommandBehavior { get; }
        IDbCommand CreateCommand();
    }
}