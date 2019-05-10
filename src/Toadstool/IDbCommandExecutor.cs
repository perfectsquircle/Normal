using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbCommandExecutor
    {
        Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> FirstAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> SingleAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
    }
}