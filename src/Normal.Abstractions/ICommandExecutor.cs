using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface ICommandExecutor
    {
        Task<IEnumerable<T>> ToEnumerableAsync<T>(CancellationToken cancellationToken = default);
        Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default);
    }

    public interface ICommandExecutor<T>
    {
        Task<IEnumerable<T>> ToEnumerableAsync(CancellationToken cancellationToken = default);
        Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default);
        Task<T> FirstAsync(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<T> SingleAsync(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync(CancellationToken cancellationToken = default);
    }
}