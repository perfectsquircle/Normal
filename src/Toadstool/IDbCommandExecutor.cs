using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbCommandExecutor
    {
        Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
    }
}