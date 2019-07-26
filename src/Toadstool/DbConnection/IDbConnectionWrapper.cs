using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDisposable
    {
        CommandBehavior CommandBehavior { get; }
        IDbCommand CreateCommand();
    }
}