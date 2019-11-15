using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IDbCommandExecutor
    {
        Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default);
    }
}