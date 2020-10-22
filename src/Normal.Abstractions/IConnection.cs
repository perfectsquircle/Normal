using System;
using System.Data.Common;

namespace Normal
{
    public interface IConnection : IDisposable
    {
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
        DbCommand CreateCommand();
    }
}