using System;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal interface IDbConnectionProvider
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
    }
}