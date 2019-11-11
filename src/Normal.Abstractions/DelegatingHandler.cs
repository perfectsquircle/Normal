using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public abstract class DelegatingHandler : IHandler
    {
        public IHandler InnerHandler { get; set; }
        public abstract Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
        public abstract Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
        public abstract Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken);
    }
}