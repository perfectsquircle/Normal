using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IDbContext : IDisposable
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
        IDbCommandBuilder CreateCommand(string commandText);
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}