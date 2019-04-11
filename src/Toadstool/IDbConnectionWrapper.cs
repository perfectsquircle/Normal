using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        CommandBehavior CommandBehavior { get; }
    }
}