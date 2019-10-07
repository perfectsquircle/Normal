using System;
using System.Data;

namespace Toadstool
{
    public interface IDbContext : IDisposable
    {
        IDbCommandBuilder CreateCommand(string commandText);
        IDbTransaction BeginTransaction();
    }
}