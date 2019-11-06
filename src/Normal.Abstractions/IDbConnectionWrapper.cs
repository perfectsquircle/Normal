using System;
using System.Data;
using System.Data.Common;

namespace Normal
{
    public interface IDbConnectionWrapper : IDisposable
    {
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
        DbCommand CreateCommand();
    }
}