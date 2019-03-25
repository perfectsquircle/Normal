using System;
using System.Data;

namespace Toadstool
{
    internal interface IDbConnectionContext : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        bool DisposeMe { get; }
    }
}