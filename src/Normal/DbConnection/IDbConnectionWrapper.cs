using System;
using System.Data.Common;

namespace Normal
{
    internal interface IDbConnectionWrapper : IDisposable
    {
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
        DbCommand CreateCommand();
    }
}