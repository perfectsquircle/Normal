using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IHandler
    {
        Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken);
        Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken);
        Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken);
    }
}