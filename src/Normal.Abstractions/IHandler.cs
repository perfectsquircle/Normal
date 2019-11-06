using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IHandler
    {
        Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
        Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
    }
}