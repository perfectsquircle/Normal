using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal interface IDbConnectionProvider : IDbTransactionWrapper
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
    }
}