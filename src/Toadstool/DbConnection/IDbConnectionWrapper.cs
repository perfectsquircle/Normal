using System;
using System.Data;

namespace Toadstool
{
    public interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbCommand CreateCommand();
    }
}