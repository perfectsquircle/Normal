using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public abstract class DelegatingHandler : IHandler
    {
        public IHandler InnerHandler { get; set; }
        public abstract Task<int> ExecuteNonQueryAsync(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
        public abstract Task<T> ExecuteScalarAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
        public abstract Task<IEnumerable<T>> ExecuteReaderAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken);
    }
}