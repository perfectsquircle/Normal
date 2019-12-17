using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal partial class DbCommandBuilder : IDbCommandExecutor
    {
        private IHandler _handler;

        internal DbCommandBuilder WithHandler(IHandler handler)
        {
            _handler = handler;
            return this;
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).Buffered();
        public async Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).ToList();
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).First();
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).FirstOrDefault();
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).Single();
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).SingleOrDefault();

        private async Task<IEnumerable<T>> ToEnumerable<T>(CancellationToken cancellationToken)
        {
            return await _handler.ExecuteReaderAsync<T>(this, cancellationToken);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        {
            return await _handler.ExecuteNonQueryAsync(this, cancellationToken);
        }

        public async Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default)
        {
            return await _handler.ExecuteScalarAsync<T>(this, cancellationToken);
        }
    }
}