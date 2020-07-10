using System;
using System.Data;

namespace Normal
{
    public interface IDatabase : IDisposable
    {
        IDbCommandBuilder CreateCommand(string commandText);
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Variant Variant { get; }
    }
}