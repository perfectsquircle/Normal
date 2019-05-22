using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContext : IDisposable
    {
        IDbCommandBuilder Command(string commandText);
        Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}