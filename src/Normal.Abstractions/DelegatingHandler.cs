using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public abstract class DelegatingHandler : IHandler
    {
        public IHandler InnerHandler { get; set; }

        public abstract Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken);

        public abstract Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken);

        public abstract Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken);
    }
}