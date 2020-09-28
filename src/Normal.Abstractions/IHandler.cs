using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IHandler
    {
        Task<int> ExecuteNonQueryAsync(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
        Task<T> ExecuteScalarAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
    }
}