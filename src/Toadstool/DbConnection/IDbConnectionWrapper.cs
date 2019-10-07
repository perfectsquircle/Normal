using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbCommand CreateCommand();
    }
}