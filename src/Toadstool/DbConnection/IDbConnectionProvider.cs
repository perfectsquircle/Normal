using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal interface IDbConnectionProvider : IDisposable
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
        Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}