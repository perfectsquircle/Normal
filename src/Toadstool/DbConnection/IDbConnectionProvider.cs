using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal interface IDbConnectionProvider
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
    }
}