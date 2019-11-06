using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class BaseHandler : IHandler
    {
        public async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}