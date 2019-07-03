using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbCommandExecutor
    {
        Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<List<dynamic>> ToListAsync(CancellationToken cancellationToken = default);
        Task<dynamic> FirstAsync(CancellationToken cancellationToken = default);
        Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<dynamic> SingleAsync(CancellationToken cancellationToken = default);
        Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default);
    }
}