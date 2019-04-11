using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContext : IDisposable
    {
        DbCommandBuilder Query(string commandText);
        Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}