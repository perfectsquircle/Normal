using System;
using System.Data;

namespace Normal
{
    public interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        IDbCommand CreateCommand();
    }
}