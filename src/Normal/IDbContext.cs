using System;
using System.Data;

namespace Normal
{
    public interface IDbContext : IDisposable
    {
        IDbCommandBuilder CreateCommand(string commandText);
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}